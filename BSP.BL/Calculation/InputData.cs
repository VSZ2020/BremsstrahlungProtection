using BSP.BL.Buildups.Common;
using BSP.BL.Materials;
using System;
using System.Collections.Generic;
using System.Text;
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

        public List<ShieldLayer> Layers;

        /// <summary>
        /// Плотность материала источника излучения (г/см^3)
        /// </summary>
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
        /// Рассчитанные потоки энергий тормозного излучения [МэВ/(с * распад)]
        /// </summary>
        public double[] BremsstrahlungFlux;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public CancellationToken CancellationToken;

        public IProgress<int> Progress;

        public SingleEnergyInputData BuildSingleEnergyInputData(int EnergyIndex)
        {
            return new SingleEnergyInputData()
            {
                Layers = Layers,
                massAttenuationFactors = massAttenuationFactors[EnergyIndex],
                SourceDensity = SourceDensity,
                IsSelfAbsorptionAllowed = IsSelfAbsorptionAllowed,
                CalculationDistance = CalculationDistance,
                BuildupProcessor = BuildupProcessor,
                BuildupFactors = BuildupFactors != null ? BuildupFactors[EnergyIndex] : new float[0][],
                CancellationToken = CancellationToken,
                Progress = Progress,
            };
        }
    }
}
