using BSP.Data.Entities;
using BSP.Data.Entities.DoseConversionFactors;

namespace BSP.DatabaseFiller.Parsers
{
    public class DoseFactorsParser
    {
        const string delimeter = "\t";

        public const string FLUX_TO_KERMA_FILE = "FluxToAirKerma.dcf";
        public const string KERMA_TO_EFF_FILE = "KermaToEffectiveDose.dcf";
        public const string KERMA_TO_EQUIV_FILE = "KermaToEquivalentDose.dcf";
        public const string KERMA_TO_EXPOSURE_FILE = "KermaToExposureDose.dcf";
        public const string KERMA_TO_ADE_FILE = "KermaToADE.dcf";
        public const string KERMA_TO_HP10_FILE = "KermaToHp10.dcf";
        public const string KERMA_TO_HP007_FILE = "KermaToHp007.dcf";

        public static string[] ExposureGeometries = ["AP", "PA", "LLAT", "RLAT", "ROT", "ISO"];

        #region ReadExposureGeometries
        public static IEnumerable<ExposureGeometryEntity> ReadExposureGeometries(int StartId = 1)
        {
            return new List<ExposureGeometryEntity>()
            {
                new ExposureGeometryEntity(){ Id = StartId++, Name = "Antero-Posterier" },
                new ExposureGeometryEntity(){ Id = StartId++, Name = "Postero-Anterier" },
                new ExposureGeometryEntity(){ Id = StartId++, Name = "Left lateral" },
                new ExposureGeometryEntity(){ Id = StartId++, Name = "Right lateral" },
                new ExposureGeometryEntity(){ Id = StartId++, Name = "Rotational" },
                new ExposureGeometryEntity(){ Id = StartId++, Name = "Isotropic" },
            };
        }
        #endregion

        #region ReadEffectiveDoseConversionFactors
        public static IEnumerable<EffectiveDoseEntity> ReadEffectiveDoseConversionFactors(int StartId = 1, string filepath = KERMA_TO_EFF_FILE)
        {
            var table = new List<float[]>();
            int exposureGeometriesCount = ExposureGeometries.Length;

            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var str_values = line.Split(delimeter);
                    table.Add(str_values.Select(float.Parse).ToArray());
                }
            }

            return MapToEffectiveDoseEntities(table.ToArray(), StartId);
        }
        #endregion

        #region ReadEquivalentDoseConversionFactors
        public static IEnumerable<EquivalentDoseEntity> ReadEquivalentDoseConversionFactors(int[] organTissueIds, int StartId = 1, string filepath = KERMA_TO_EQUIV_FILE)
        {
            var DCFs = new List<EquivalentDoseEntity>();

            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith("REGION FACTORS")) { }

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("BEGIN GEOMETRY"))
                    {
                        var geometryId = int.Parse(line.Split("=")[1].Trim());
                        var entities = ReadEquivalentDoseEntitiesBlockForGeometry(reader, geometryId, organTissueIds, StartId);
                        StartId += entities.Count;
                        DCFs.AddRange(entities);
                    }

                }
            }

            return DCFs;
        }
        #endregion

        #region ReadEquivalentDoseEntitiesBlockForGeometry
        private static List<EquivalentDoseEntity> ReadEquivalentDoseEntitiesBlockForGeometry(StreamReader reader, int geometryId, int[] organTissueIds, int StartId = 1)
        {
            var DCFs = new List<EquivalentDoseEntity>();
            string line = "";
            while (!(line = reader.ReadLine()).StartsWith("END"))
            {
                if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                    continue;
                var str_values = line.Split(delimeter);
                var parsed_values = str_values.Select(float.Parse).ToArray();
                var energy = parsed_values[0];
                for (var i = 1; i < parsed_values.Length; i++)
                {
                    DCFs.Add(new EquivalentDoseEntity()
                    {
                        Id = StartId++,
                        Energy = energy,
                        Value = parsed_values[i],
                        ExposureGeometryId = geometryId,
                        OrganTissueId = organTissueIds[i - 1]
                    });
                }
            }
            return DCFs;
        }
        #endregion

        #region ReadOrgansAndTissues
        public static List<OrganTissueEntity> ReadOrgansAndTissues(int StartId = 1, string filepath = KERMA_TO_EQUIV_FILE)
        {
            var organs = new List<OrganTissueEntity>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while (!reader.ReadLine().StartsWith("REGION ORGANS AND TISSUES")) { }

                while (!(line = reader.ReadLine()).StartsWith("ENDREGION"))
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var str_values = line.Split(delimeter);
                    var id = int.Parse(str_values[0]);
                    var organ_name = str_values[1];

                    organs.Add(new OrganTissueEntity()
                    {
                        Id = id + StartId - 1,
                        Name = organ_name
                    });
                }
            }
            return organs;
        }
        #endregion

        #region MapToEffectiveDoseEntities
        private static List<EffectiveDoseEntity> MapToEffectiveDoseEntities(float[][] values, int StartId = 1)
        {
            var DCFs = new List<EffectiveDoseEntity>();
            for (var i = 0; i < ExposureGeometries.Length; i++)
            {
                for (var j = 0; j < values.Length; j++)
                {
                    var exposure_geometry_index = i + 1;
                    var dcf = new EffectiveDoseEntity()
                    {
                        Id = StartId++,
                        Energy = values[j][0],
                        Value = values[j][exposure_geometry_index],
                        ExposureGeometryId = exposure_geometry_index,
                    };
                    DCFs.Add(dcf);
                }
            }
            return DCFs;
        }
        #endregion

        #region Air Kerma, H*(10), Hp(10), Hp(0.07), X
        public static IEnumerable<AirKermaEntity> ReadAirKermaDoseConversionFactors(int StartIndex = 1, string filepath = FLUX_TO_KERMA_FILE) => ReadSimpleDoseConversionFactors<AirKermaEntity>(filepath, StartIndex);

        public static IEnumerable<ExposureDoseEntity> ReadExposureDoseConversionFactors(int StartIndex = 1, string filepath = KERMA_TO_EXPOSURE_FILE) => ReadSimpleDoseConversionFactors<ExposureDoseEntity>(filepath, StartIndex);

        public static IEnumerable<AmbientDoseEquivalentEntity> ReadAmbientDoseConversionFactors(int StartIndex = 1, string filepath = KERMA_TO_ADE_FILE) => ReadSimpleDoseConversionFactors<AmbientDoseEquivalentEntity>(filepath, StartIndex);

        public static IEnumerable<Hp10Entity> ReadHp10DoseConversionFactors(int StartIndex = 1, string filepath = KERMA_TO_HP10_FILE) => ReadSimpleDoseConversionFactors<Hp10Entity>(filepath, StartIndex);

        public static IEnumerable<Hp007Entity> ReadHp007DoseConversionFactors(int StartIndex = 1, string filepath = KERMA_TO_HP007_FILE) => ReadSimpleDoseConversionFactors<Hp007Entity>(filepath, StartIndex);
        #endregion

        #region ReadSimpleDoseConversionFactors
        public static IEnumerable<TFactor> ReadSimpleDoseConversionFactors<TFactor>(string filepath, int StartIndex = 1) where TFactor : BaseDoseFactorEntity, new()
        {
            var DCFs = new List<TFactor>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;

                    var str_values = line.Split(delimeter);
                    var values = str_values.Select(float.Parse).ToArray();
                    DCFs.Add(new TFactor()
                    {
                        Id = StartIndex++,
                        Energy = values[0],
                        Value = values[1]
                    });
                }
            }

            return DCFs;
        }
        #endregion
    }
}
