using BSP.Common;
using System.ComponentModel;
using System.Windows;

namespace BSP.ViewModels
{
    public class BremsstrahlungEnergyYieldVM : BaseViewModel, IDataErrorInfo
    {
        private float energy;
        private double energyYield;
        private double energyFlux;

        /// <summary>
        /// Средняя энергия группы в [МэВ]
        /// </summary>
        public float Energy { get => energy; set { energy = value; OnChanged(); } }

        /// <summary>
        /// Выход энергии тормозного излучения для группы в [МэВ/распад]
        /// </summary>
        public double EnergyYield { get => energyYield; set { energyYield = value; OnChanged(); } }

        /// <summary>
        /// Поток энергии тормозного излучения
        /// </summary>
        public double EnergyFlux { get => energyFlux;  set{ energyFlux = value; OnChanged(); }  }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Energy):
                        if (Energy < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationEnergy") as string) ?? "Incorrect value";
                        break;
                    case nameof(EnergyYield):
                        if (EnergyYield < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationEnergyYield") as string) ?? "Incorrect value";
                        break;
                    case nameof(EnergyFlux):
                        if (EnergyYield < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationEnergyYield") as string) ?? "Incorrect value";
                        break;
                }
                return error;
            }
        }
        public string Error => throw new NotImplementedException();
    }
}
