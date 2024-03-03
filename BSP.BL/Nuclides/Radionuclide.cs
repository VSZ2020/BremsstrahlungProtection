using System.Linq;

namespace BSP.BL.Nuclides
{
    public class Radionuclide
    {
        public Radionuclide()
        {

            //RecalculateMeanEnergies();
        }

        /// <summary>
        /// Название радионуклида
        /// </summary>
        public string Name { get; set; } = "Безымянный нуклид";


        /// <summary>
        /// Период полураспада
        /// </summary>
        public double HalfLive { get; set; }

        public double HalfLiveUnits { get; set; }

        public float[] MaxEnergies;        //Набор максимальных энергий нуклида, [МэВ]
        public float[] MeanEnergies;       //Набор средних энергий нуклида, [МэВ/част.]
        public float[] EnergyYields;       //Набор интенсивности линии излучения, [доли]



        public float[] EnergyByIntensityArray => Enumerable.Range(0, EnergyYields.Length).Select(i => MaxEnergies[i] * EnergyYields[i]).ToArray();

        public float GetEnergyWithMaxIntensity()
        {
            float maxEnergy = 0.0F;
            float maxEI = MaxEnergies[0] * EnergyYields[0];
            for (var i = 1; i < MaxEnergies.Length; i++)
            {
                if (MaxEnergies[i] * EnergyYields[i] > maxEI)
                {
                    maxEI = MaxEnergies[i] * EnergyYields[i];
                    maxEnergy = MaxEnergies[i];
                }
            }
            return maxEnergy;
        }
        ///// <summary>
        ///// Пересчитывает групповые энергии тормозного излучения для данного нуклида
        ///// </summary>
        //public void RecalculateMeanEnergies()
        //{
        //    double maxEnergy = 0.0;

        //    //Поиск максимальной линии энергии с максимальным вкладом
        //    for (int i = 0; i < MaxEnergies.Count; i++)
        //    {
        //        if (MaxEnergies[i] * EnergyYields[i] > maxEnergy)
        //            maxEnergy = MaxEnergies[i];
        //    }

        //    var binRightEdges = Bremsstrahlung.GetEnergyBinRightEdges(maxEnergy);
        //    BSEnergySpectrum = new BsSpectrum(binRightEdges.Length);
        //    BSEnergySpectrum.MeanEnergies = Bremsstrahlung.GetMeanEnergyBins(binRightEdges);
        //}
    }
}
