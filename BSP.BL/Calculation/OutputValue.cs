using System.Linq;

namespace BSP.BL.Calculation
{
    public class OutputValue
    {

        /// <summary>
        /// Возвращает суммарную мощность дозы
        /// </summary>
        public double TotalDoseRate => PartialDoseRates != null ? PartialDoseRates.Sum() : 0;

        /// <summary>
        /// Массив с парциальными мощностями доз (по каждой энергетической группе тормозного излучения)
        /// </summary>
        public double[] PartialDoseRates;

    }
}
