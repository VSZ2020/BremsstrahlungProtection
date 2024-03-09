using System.Linq;

namespace BSP.BL.Calculation
{
    public class OutputValue
    {

        /// <summary>
        /// Возвращает суммарную мощность дозы
        /// </summary>
        public double TotalDoseRate => PartialAirKerma != null ? PartialAirKerma.Sum() : 0;

        /// <summary>
        /// Массив с парциальными мощностями доз (по каждой энергетической группе тормозного излучения)
        /// </summary>
        public double[] PartialAirKerma;

        public double[] PartialEnergyFluxDensity;
        
        public double[] ConvertTo(double[] doseFactors)
        {
            return Enumerable.Range(0, PartialAirKerma.Length).Select(i => PartialAirKerma[i] * doseFactors[i]).ToArray();
        }
    }
}
