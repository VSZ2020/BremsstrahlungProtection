using BSP.Geometries.SDK;
using System.Numerics;

namespace BSP.BL.Calculation
{
    /// <summary>
    /// Класс с входными данными для геометрии источника. Содержит данные для всех энергетических групп
    /// </summary>
    public class InputData
    {
        public float[] Dimensions;
        public int[] Discreteness;

        public double[] Energies;
        public double[] massEnvironmentAbsorptionFactors;
        public double[][] massAttenuationFactors;
        public double[][][] BuildupFactors;

        public List<ShieldLayer> Layers;

        /// <summary>
        /// Плотность материала источника излучения (г/см^3)
        /// </summary>
        public float SourceDensity = 0;

        public double SourceActivity = 0;

        /// <summary>
        /// Флаг учета самопоглощения в материале источника
        /// </summary>
        public bool IsSelfAbsorptionAllowed = true;

        /// <summary>
        /// Координаты точки регистрации излучения
        /// </summary>
        public Vector3 CalculationPoint;

        /// <summary>
        /// Рассчитанные потоки фотоквантов [фотон/с]
        /// </summary>
        public double[] PhotonsFluxes;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public CancellationToken CancellationToken;

        public IProgress<double> Progress;

        public SingleEnergyInputData BuildSingleEnergyInputData(int EnergyIndex)
        {
            return new SingleEnergyInputData()
            {
                Dimensions = this.Dimensions,
                Discreteness = this.Discreteness,
                Layers = Layers,
                MassAttenuationFactors = massAttenuationFactors[EnergyIndex],
                SourceDensity = SourceDensity,
                CalculationPoint = CalculationPoint,
                BuildupProcessor = BuildupProcessor,
                BuildupFactors = BuildupFactors != null ? BuildupFactors[EnergyIndex] : new double[0][],
                CancellationToken = CancellationToken,
                Progress = Progress,
                IsSelfAbsorptionAllowed = IsSelfAbsorptionAllowed,
            };
        }
    }
}
