namespace BSP.BL.Nuclides
{
    public class Radionuclide
    {
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
    }
}
