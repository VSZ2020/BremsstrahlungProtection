using BSP.BL.Buildups.Common;
using BSP.BL.Materials;

namespace BSP.BL.Calculation
{
    /// <summary>
    /// Файл с входными параметрами для геометрии для фиксированной энергии излучения
    /// </summary>
    public class SingleEnergyInputData
    {
        public double[] massAttenuationFactors;
        public double[][] BuildupFactors;

        public List<ShieldLayer> Layers;

        public float SourceDensity = 0;

        /// <summary>
        /// Флаг учета самопоглощения в материале источника
        /// </summary>
        public bool IsSelfAbsorptionAllowed = true;

        /// <summary>
        /// Расстояние от точечного источника до точки регистрации излучения
        /// </summary>
        public double CalculationDistance = 1;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public IProgress<int> Progress;
        public CancellationToken CancellationToken;
    }
}
