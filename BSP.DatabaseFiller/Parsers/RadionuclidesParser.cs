using BSP.Data.Entities.Radionuclides;

namespace BSP.DatabaseFiller.Parsers
{
    public class RadionuclidesParser
    {
        const string delimeter = "\t";

        #region GetRadionuclidesFilepaths
        public static string[] GetRadionuclidesFilepaths(string directory)
        {
            return Directory.EnumerateFiles(directory).Where(f => f.EndsWith(".rd") && !Path.GetFileNameWithoutExtension(f).StartsWith("#")).ToArray();
        }
        #endregion

        #region ReadRadionuclidesInfo
        public static List<RadionuclideEntity> ReadRadionuclidesInfo(string[] filepaths, int StartId = 1)
        {
            return Enumerable.Range(0, filepaths.Length).Select(i => ParseRadionuclideInfo(filepaths[i], StartId + i)).ToList();
        }
        #endregion

        #region ParseRadionuclideInfo
        private static RadionuclideEntity ParseRadionuclideInfo(string filepath, int StartId = 1)
        {
            var entity = new RadionuclideEntity();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!(line = reader.ReadLine()).StartsWith("RADIONUCLIDE")) { }

                var str_values = reader.ReadLine().Split(delimeter);

                entity.Id = StartId++;
                entity.Name = Path.GetFileNameWithoutExtension(filepath);
                entity.Z = float.Parse(str_values[0]);
                entity.Weight = float.Parse(str_values[1]);
                entity.HalfLife = float.Parse(str_values[2]);
                entity.HalfLiveUnits = str_values[3];

            }
            return entity;
        }
        #endregion


        #region ReadEnergyIntensityData
        public static IEnumerable<RadionuclideEnergyIntensityEntity> ReadEnergyIntensityData(string filepath, int radionuclideId, int StartId = 1)
        {
            var factors = new List<RadionuclideEnergyIntensityEntity>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith("BETA_ENERGY_INTENSITY")) { }

                while (!(line = reader.ReadLine()).StartsWith("END"))
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var values = line.Split(delimeter).Select(float.Parse).ToArray();
                    factors.Add(new RadionuclideEnergyIntensityEntity()
                    {
                        Id = StartId++,
                        RadionuclideId = radionuclideId,
                        EndpointEnergy = values[0],
                        AverageEnergy = values[1],
                        Yield = values[2],
                    });
                }
            }
            return factors;
        }
        #endregion
    }
}
