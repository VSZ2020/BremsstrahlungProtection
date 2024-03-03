using BSP.BL.Buildups.Common;
using BSP.BL.Materials;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BSP.BL.Calculation
{
    /// <summary>
    /// Класс с входными данными для геометрии источника. Содержит данные для всех энергетических групп
    /// </summary>
    public class InputData
    {
        public float[] massEnvironmentAbsorptionFactors;
        public float[][] massAttenuationFactors;
        public float[][][] BuildupFactors;
        public float[] DoseConversionFactors;

        public List<ShieldLayer> Layers;

        public double SourceActivity = 0;

        public float SourceDensity = 0;

        /// <summary>
        /// Флаг учета самопоглощения в материале источника
        /// </summary>
        public bool IsSelfAbsorptionAllowed = false;

        /// <summary>
        /// Расстояние от точечного источника до точки регистрации излучения
        /// </summary>
        public double CalculationDistance = 1;

        /// <summary>
        /// Рассчитанные выходы энергий тормозного излучения [МэВ/распад]
        /// </summary>
        public double[] BremsstrahlungYields;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public IProgress<int> Progress;

        public CancellationToken CancellationToken;

        public SingleEnergyInputData BuildSingleEnergyInputData(int EnergyIndex)
        {
            return new SingleEnergyInputData()
            {
                massAttenuationFactors = massAttenuationFactors[EnergyIndex],
                Layers = Layers,
                SourceDensity = SourceDensity,
                IsSelfAbsorptionAllowed = IsSelfAbsorptionAllowed,
                CalculationDistance = CalculationDistance,
                BuildupProcessor = BuildupProcessor,
                BuildupFactors = BuildupFactors != null ? BuildupFactors[EnergyIndex] : new float[0][],
                Progress = Progress,
                CancellationToken = CancellationToken
            };
        }
    }
}
