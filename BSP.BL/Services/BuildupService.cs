using BSP.BL.Buildups;
using BSP.BL.Buildups.Common;
using BSP.BL.Interpolation;
using BSP.BL.Interpolation.Functions;
using BSP.Data;
using BSP.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSP.BL.Services
{
    public class BuildupService
    {
        public BuildupService(DataContext ctx)
        {
            this.context = ctx;
        }

        private readonly DataContext context;

        private static Dictionary<Type, string> _availableHeterogeneousBuildups = new Dictionary<Type, string>()
        {
            { typeof(BuildupBroder), "Broder" },
        };

        private static Dictionary<Type, string> _availableHomogeneousBuildups = new Dictionary<Type, string>()
        {
            {typeof(BuildupGeometricProgression), "Geometric Progression" },
            {typeof(BuildupTaylor), "Taylor 2-EXP" },
        };

        public static Dictionary<Type, string> GetHeterogeneousBuildups => _availableHeterogeneousBuildups;

        public static Dictionary<Type, string> GetHomogeneousBuildups => _availableHomogeneousBuildups;

        public static BaseHeterogeneousBuildup GetHeterogeneousBuildupInstance(Type heteroBuildupType, Type homogenBuildupType)
        {
            var homogeneousBuildup = GetHomogeneousBuildupInstance(homogenBuildupType);
            if (homogeneousBuildup == null)
                throw new NullReferenceException($"Can't create instance of {nameof(homogenBuildupType)}");

            if (heteroBuildupType == typeof(BuildupBroder))
                return new BuildupBroder(homogeneousBuildup.EvaluateBuildup);


            return new BuildupBroder(homogeneousBuildup.EvaluateBuildup);
            //return Activator.CreateInstance(heteroBuildupType, new []{homogeneousBuildup.EvaluateBuildup}) as BaseHeterogeneousBuildup;
        }

        public static BaseBuildup GetHomogeneousBuildupInstance(Type homogenBuildupType)
        {
            return Activator.CreateInstance(homogenBuildupType) as BaseBuildup;
            //if (homogenBuildupType == typeof(BuildupTaylor))
            //    return new BuildupTaylor();

            //if (homogenBuildupType == typeof(BuildupGeometricProgression))
            //    return new BuildupGeometricProgression();
        }

        /// <summary>
        /// Возвращает ключ словаря, в котором содержится название для текущей локализации приложения
        /// </summary>
        /// <param name="heteroBuildupType">Тип фактора накопления для гетерогенной среды</param>
        /// <returns></returns>
        public static string GetTranslationKeyHeterogeneousBuildup(Type heteroBuildupType)
        {
            return heteroBuildupType switch
            {
                //Type e when e == typeof(BuildupBroder) => "BuildupNameBroder",
                _ => null
            };
        }

        /// <summary>
        /// Возвращает ключ словаря, в котором содержится название для текущей локализации приложения
        /// </summary>
        /// <param name="homogenBuildupType">Тип фактора накопления для гомогенной среды</param>
        /// <returns></returns>
        public static string GetTranslationKeyHomogeneousBuildup(Type homogenBuildupType)
        {
            return homogenBuildupType switch
            {
                Type e when e == typeof(BuildupGeometricProgression) => "BuildupNameGeometricProgression",
                Type e when e == typeof(BuildupTaylor) => "BuildupNameTaylor2EXP",
                _ => null
            };
        }

        public static string[] GetBuildupCoefficientsNames(Type selectedHomogeneousBuildupType)
        {
            if (selectedHomogeneousBuildupType == typeof(BuildupTaylor))
                return ["A1", "Alpha1", "Alpha2", "Delta"];

            return ["A", "B", "C", "D", "Xi", "Delta"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildupType"></param>
        /// <param name="materialId"></param>
        /// <returns>Массив энергий и массив табличных коэффициентов по каждой энергии. Структура второго массива: float[EnergyIndex][CoefficientIndex]</returns>
        public (float[] energies, float[][] values) GetTableFactors(Type buildupType, int materialId)
        {
            var table_entities = new List<BaseMaterialFactorEntity>();
            if (buildupType == typeof(BuildupTaylor))
                table_entities.AddRange(context.Taylor2ExpFactors.AsNoTracking().Where(e => e.MaterialId == materialId).ToList());

            if (buildupType == typeof(BuildupGeometricProgression))
                table_entities.AddRange(context.GeometricProgressionFactors.AsNoTracking().Where(e => e.MaterialId == materialId).ToList());

            var table_energies = table_entities.Select(e => e.Energy).ToArray();
            var table_coeffs = table_entities.Select(e => e.Values).ToArray();

            return (table_energies, table_coeffs);
        }


        public float[][][] GetInterpolatedBuildupFactors(Type buildupType, int[] materialsIds, float[] energies, InterpolationType interpolatorType = InterpolationType.Linear)
        {
            var coefficientsCount = buildupType == typeof(BuildupTaylor) ? 4 : 6;
            //Пустой массив для заполнения интерполированными значениями
            var outputArray = InitializeArray(energies.Length, materialsIds.Length, coefficientsCount);

            for (var i = 0; i < materialsIds.Length; i++)
            {
                (var table_energies, var table_coeffs) = GetTableFactors(buildupType, materialsIds[i]);

                //Транспонируем матрицу коэффициентов. Теперь структура индексов следующая: float[CoefficientIndex][EnergyIndex]
                var transposedTableCoeffs = TransposeCoefficientsArray(table_coeffs, coefficientsCount);

                for (var j = 0; j < transposedTableCoeffs.Length; j++)
                {
                    //Интерполируем значения для каждого типа коэффициента
                    var interpolatedCoeffs = Interpolator.Interpolate(table_energies, transposedTableCoeffs[j], energies, interpolatorType);

                    //Заполняем массив выходных значений по энергиям
                    for (var k = 0; k < energies.Length; k++)
                        outputArray[k][i][j] = interpolatedCoeffs[k];
                }
            }

            return outputArray;
        }

        private float[][] TransposeCoefficientsArray(float[][] arr, int coefficientsCount)
        {
            int energiesCount = arr.Length;
            float[][] newArr = new float[coefficientsCount][];
            for (var i = 0; i < coefficientsCount; i++)
            {
                newArr[i] = new float[energiesCount];
                for (var j = 0; j < energiesCount; j++)
                    newArr[i][j] = arr[j][i];
            }
            return newArr;
        }

        private float[][][] InitializeArray(int n, int m, int k)
        {
            var array = new float[n][][];
            for (int i = 0; i < n; i++)
            {
                array[i] = new float[m][];
                for (int j = 0; j < m; j++)
                    array[i][j] = new float[k];
            }
            return array;
        }
    }
}
