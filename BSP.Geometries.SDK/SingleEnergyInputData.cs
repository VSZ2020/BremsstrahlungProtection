using System.Numerics;

namespace BSP.Geometries.SDK
{
    /// <summary>
    /// Файл с входными параметрами для геометрии для фиксированной энергии излучения
    /// </summary>
    public class SingleEnergyInputData
    {
        public float[] Dimensions;
        public int[] Discreteness;
        
        public double[] MassAttenuationFactors;
        public double[][] BuildupFactors;

        public List<ShieldLayer> Layers;

        public float SourceDensity = 0;

        /// <summary>
        /// Флаг учета самопоглощения в материале источника
        /// </summary>
        public bool IsSelfAbsorptionAllowed = true;

        /// <summary>
        /// Коорлинаты точки регистрации излучения
        /// </summary>
        public Vector3 CalculationPoint;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public IProgress<double> Progress;
        public CancellationToken CancellationToken;
    }
}
