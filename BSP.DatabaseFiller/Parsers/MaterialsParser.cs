using BSP.Data.Entities;
using BSP.Data.Entities.MaterialFactors;

namespace BSP.DatabaseFiller.Parsers
{
    public class MaterialsParser
    {
        const string delimeter = "\t";

        #region GetMaterialsFilepaths
        public static string[] GetMaterialsFilepaths(string directory)
        {
            return Directory.EnumerateFiles(directory).Where(f => f.EndsWith(".md") && !Path.GetFileNameWithoutExtension(f).StartsWith("#")).ToArray();
        }
        #endregion

        #region ReadMaterials
        public static List<MaterialEntity> ReadMaterials(string[] filepaths, int StartId = 1)
        {
            return Enumerable.Range(0, filepaths.Length).Select(i => ParseMaterialInfo(filepaths[i], StartId + i)).ToList();
        }
        #endregion

        #region ParseMaterialInfo
        private static MaterialEntity ParseMaterialInfo(string filepath, int StartId = 1)
        {
            MaterialEntity entity = new MaterialEntity();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith("MATERIAL")) { }
                var str_values = reader.ReadLine().Split(delimeter);
                var values = str_values.Select(float.Parse).ToArray();

                entity.Id = StartId++;
                entity.Name = Path.GetFileNameWithoutExtension(filepath);
                entity.Z = values[0];
                entity.Weight = values[1];
                entity.Density = values[2];
                entity.MassFraction = values[3];
            }
            return entity;
        }
        #endregion


        #region Read Absorption and Attenuation factors
        public static IEnumerable<AbsorptionFactorEntity> ReadAbsorptionFactors(string filepath, int materialId, int StartId = 1)
            => ReadSimpleAbsorptionAndAttenuationFactors<AbsorptionFactorEntity>(filepath, materialId, "MASS_ABSORPTION_FACTORS", StartId);

        public static IEnumerable<AttenuationFactorEntity> ReadAttenuationFactors(string filepath, int materialId, int StartId = 1)
            => ReadSimpleAbsorptionAndAttenuationFactors<AttenuationFactorEntity>(filepath, materialId, "MASS_ATTENUATION_FACTORS", StartId);
        #endregion

        #region ReadSimpleAbsorptionAndAttenuationFactors
        private static IEnumerable<TFactor> ReadSimpleAbsorptionAndAttenuationFactors<TFactor>(string filepath, int materialId, string region_keyword, int StartId = 1) where TFactor : BaseMaterialFactorEntity, new()
        {
            var factors = new List<TFactor>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith(region_keyword)) { }

                while (!(line = reader.ReadLine()).StartsWith("END"))
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var values = line.Split(delimeter).Select(float.Parse).ToArray();
                    var factor = new TFactor()
                    {
                        Id = StartId++,
                        Energy = values[0],
                        MaterialId = materialId,
                    };
                    if (typeof(TFactor) == typeof(AbsorptionFactorEntity))
                        (factor as AbsorptionFactorEntity).Value = values[1];

                    if (typeof(TFactor) == typeof(AttenuationFactorEntity))
                        (factor as AttenuationFactorEntity).Value = values[1];

                    factors.Add(factor);
                }
            }
            return factors;
        }
        #endregion


        #region ReadTaylorCoefficients
        public static IEnumerable<Taylor2ExpEntity> ReadTaylorCoefficients(string filepath, int materialId, int StartId = 1)
        {
            var factors = new List<Taylor2ExpEntity>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith("TAYLOR2EXP_BUILDUP_FACTORS")) { }

                while (!(line = reader.ReadLine()).StartsWith("END"))
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var values = line.Split(delimeter).Select(float.Parse).ToArray();
                    factors.Add(new Taylor2ExpEntity()
                    {
                        Id = StartId++,
                        MaterialId = materialId,
                        Energy = values[0],
                        A = values[1],
                        Alpha1 = values[2],
                        Alpha2 = values[3],
                        BarrierFactor = values[4]
                    });
                }
            }
            return factors;
        }
        #endregion

        #region ReadGeometricProgressionCoefficients
        public static IEnumerable<GeometricProgressionEntity> ReadGeometricProgressionCoefficients(string filepath, int materialId, int StartId = 1)
        {
            var factors = new List<GeometricProgressionEntity>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith("GEOMETRIC_PROGRESSION_FACTORS")) { }

                while (!(line = reader.ReadLine()).StartsWith("END"))
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var values = line.Split(delimeter).Select(float.Parse).ToArray();
                    factors.Add(new GeometricProgressionEntity()
                    {
                        Id = StartId++,
                        MaterialId = materialId,
                        Energy = values[0],
                        a = values[1],
                        b = values[2],
                        c = values[3],
                        d = values[4],
                        xi = values[5],
                        BarrierFactor = values[6]
                    });
                }
            }
            return factors;
        }
        #endregion
    }
}
