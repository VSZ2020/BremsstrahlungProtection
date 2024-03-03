using BSP.Common;

namespace BSP.ViewModels
{
    public class BremsstrahlungEnergyYieldVM : BaseViewModel
    {
        private float energy;
        private double energyYield;

        /// <summary>
        /// Средняя энергия группы в [МэВ]
        /// </summary>
        public float Energy { get => energy; set { energy = value; OnChanged(); } }

        /// <summary>
        /// Выход энергии томозного излучения для группы в [МэВ/распад]
        /// </summary>
        public double EnergyYield { get => energyYield; set { energyYield = value; OnChanged(); } }
    }
}
