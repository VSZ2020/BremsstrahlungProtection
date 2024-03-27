using BSP.BL.Buildups;
using BSP.BL.Buildups.Common;
using BSP.BL.Extensions;
using BSP.BL.Interpolation;
using BSP.BL.Interpolation.Functions;
using BSP.Data;
using BSP.Data.Entities;
using BSP.Geometries.SDK;
using Microsoft.EntityFrameworkCore;

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
            { typeof(BuildupLastLayer), "Last layer" },
            { typeof(BuildupAverage), "Weighted average" }
        };

        private static Dictionary<Type, string> _availableHomogeneousBuildups = new Dictionary<Type, string>()
        {
            {typeof(BuildupImprovedGeometricProgression), "Improved Geometric Progression" },
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

            if (heteroBuildupType == typeof(BuildupLastLayer))
                return new BuildupLastLayer(homogeneousBuildup.EvaluateBuildup);

            if (heteroBuildupType == typeof(BuildupAverage))
                return new BuildupAverage(homogeneousBuildup.EvaluateBuildup);


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
                Type e when e == typeof(BuildupImprovedGeometricProgression) => "BuildupNameImprovedGeometricProgression",
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
        public (double[] energies, double[][] values) GetTableFactors(Type buildupType, int materialId)
        {
            var table_entities = new List<BaseMaterialFactorEntity>();
            if (buildupType == typeof(BuildupTaylor))
                table_entities.AddRange(context.Taylor2ExpFactors.AsNoTracking().Where(e => e.MaterialId == materialId).ToList());

            if (buildupType == typeof(BuildupGeometricProgression) || buildupType == typeof(BuildupImprovedGeometricProgression))
                table_entities.AddRange(context.GeometricProgressionFactors.AsNoTracking().Where(e => e.MaterialId == materialId).ToList());

            var table_energies = table_entities.Select(e => (double)e.Energy).ToArray();
            var table_coeffs = table_entities.Select(e => e.Values.Select(v => (double)v).ToArray()).ToArray();

            return (table_energies, table_coeffs);
        }


        public double[] GetInterpolatedBuildupFactors(double[] tableEnergies, double[] tableValues, double[] energies, InterpolationType interpolatorType = InterpolationType.Linear)
        {
            return Interpolator.Interpolate(tableEnergies, tableValues, energies, interpolatorType, AxisLogScale.OnlyX);
        }

        public double[][][] GetInterpolatedBuildupFactors(Type buildupType, int[] materialsIds, double[] energies, InterpolationType interpolatorType = InterpolationType.Linear)
        {
            var energiesLog10 = energies.ToLog10();
            var coefficientsCount = buildupType == typeof(BuildupTaylor) ? 4 : 6;
            
            //Пустой массив для заполнения интерполированными значениями
            var outputArray = InitializeArray(energies.Length, materialsIds.Length, coefficientsCount);

            for (var i = 0; i < materialsIds.Length; i++)
            {
                (var table_energies, var table_coeffs) = GetTableFactors(buildupType, materialsIds[i]);

                //var tableEnergiesLog10 = table_energies.ToLog10();

                for (var j = 0; j < coefficientsCount; j++)
                {
                    var coeffs = table_coeffs.Select(c => c[j]).ToArray();
                    //Интерполируем значения для каждого типа коэффициента
                    var interpolatedCoeffs = Interpolator.Interpolate(table_energies, coeffs, energies, interpolatorType);

                    //Заполняем массив выходных значений по энергиям
                    for (var k = 0; k < energies.Length; k++)
                        outputArray[k][i][j] = interpolatedCoeffs[k];
                }
            }

            return outputArray;
        }

        private double[][][] InitializeArray(int n, int m, int k)
        {
            var array = new double[n][][];
            for (int i = 0; i < n; i++)
            {
                array[i] = new double[m][];
                for (int j = 0; j < m; j++)
                    array[i][j] = new double[k];
            }
            return array;
        }
    }
}
