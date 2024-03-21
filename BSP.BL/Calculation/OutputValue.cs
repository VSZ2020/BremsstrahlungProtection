using System.Linq;
using System.Numerics;

namespace BSP.BL.Calculation
{
    public class OutputValue
    {
        public OutputValue(int energiesCount)
        {
            Energies = new double[energiesCount];
            PartialPhotonsFlux = new double[energiesCount];
            PartialFluxDensity = new double[energiesCount];
            PartialAirKerma = new double[energiesCount];
            DosePoint = new Vector3();
        }

        public Vector3 DosePoint;

        /// <summary>
        /// Возвращает суммарную мощность дозы
        /// </summary>
        public double TotalDoseRate => PartialAirKerma != null ? PartialAirKerma.Sum() : 0;

        public double[] Energies;

        /// <summary>
        /// Массив парциальных потоков фотонов [фотон/с]
        /// </summary>
        public double[] PartialPhotonsFlux;

        /// <summary>
        /// Массив с парциальными мощностями доз (по каждой энергетической группе тормозного излучения)
        /// </summary>
        public double[] PartialAirKerma;

        /// <summary>
        /// Массив с парциальными значениями плотности потока кнватов тормозного излучения
        /// </summary>
        public double[] PartialFluxDensity;
        
        public double[] ConvertToAnotherDose(double[] doseFactors)
        {
            return Enumerable.Range(0, PartialAirKerma.Length).Select(i => PartialAirKerma[i] * doseFactors[i]).ToArray();
        }
    }
}
