using BSP.Data;
using BSP.DatabaseFiller.Parsers;
using Microsoft.EntityFrameworkCore;

namespace BSP.DatabaseFiller
{
    public class DatabaseFiller
    {
        public DatabaseFiller()
        {
            context = new DataContext(new DbContextOptionsBuilder().UseSqlite(CONNECTION_STRING).Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private readonly DataContext context;

        public const string CONNECTION_STRING = "Data Source=Database.mdb";

        public void FillDatabaseWithDoseFactors(string doseFactorsFolder)
        {
            var exposureGeometries = DoseFactorsParser.ReadExposureGeometries();
            var organsTissues = DoseFactorsParser.ReadOrgansAndTissues(1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_EQUIV_FILE));

            var airKermaDCFs = DoseFactorsParser.ReadAirKermaDoseConversionFactors(1, Path.Combine(doseFactorsFolder, DoseFactorsParser.FLUX_TO_KERMA_FILE));
            var AdeDCFs = DoseFactorsParser.ReadAmbientDoseConversionFactors(airKermaDCFs.Last().Id + 1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_ADE_FILE));
            var exposureDCFs = DoseFactorsParser.ReadExposureDoseConversionFactors(AdeDCFs.Last().Id + 1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_EXPOSURE_FILE));
            var Hp10DCFs = DoseFactorsParser.ReadHp10DoseConversionFactors(exposureDCFs.Last().Id + 1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_HP10_FILE));
            var Hp007DCFs = DoseFactorsParser.ReadHp007DoseConversionFactors(Hp10DCFs.Last().Id + 1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_HP007_FILE));
            var effectiveDCFs = DoseFactorsParser.ReadEffectiveDoseConversionFactors(Hp007DCFs.Last().Id + 1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_EFF_FILE));
            var equivalentDCFs = DoseFactorsParser.ReadEquivalentDoseConversionFactors(organsTissues.Select(o => o.Id).ToArray(), effectiveDCFs.Last().Id + 1, Path.Combine(doseFactorsFolder, DoseFactorsParser.KERMA_TO_EQUIV_FILE));

            context.ExposureGeometries.AddRange(exposureGeometries);
            context.OrgansAndTissues.AddRange(organsTissues);
            context.AirKermaDoseFactors.AddRange(airKermaDCFs);
            context.AmbientDoseEquivalentFactors.AddRange(AdeDCFs);
            context.ExposureDoseFactors.AddRange(exposureDCFs);
            context.Hp10Factors.AddRange(Hp10DCFs);
            context.Hp007Factors.AddRange(Hp007DCFs);
            context.EffectiveDoseFactors.AddRange(effectiveDCFs);
            context.EquivalentDoseFactors.AddRange(equivalentDCFs);

            context.SaveChanges();
        }

        public void FillDatabaseWithMaterialsData(string materialsDataFolder)
        {
            var paths = MaterialsParser.GetMaterialsFilepaths(materialsDataFolder);

            var materials = MaterialsParser.ReadMaterials(paths);
            Console.WriteLine($"\tWrite materials data. Found {materials.Count} materials");
            context.Materials.AddRange(materials);

            int startId = 1;
            for (var i = 0; i < materials.Count; i++)
            {
                Console.WriteLine($"\tWrite absoprtion factors for {materials[i].Name}");
                var absorptionFactors = MaterialsParser.ReadAbsorptionFactors(paths[i], materials[i].Id, startId);
                context.MassAbsorptionFactors.AddRange(absorptionFactors);
                startId += absorptionFactors.Count();

                Console.WriteLine($"\tWrite attenuation factors for {materials[i].Name}");
                var attenuationFactors = MaterialsParser.ReadAttenuationFactors(paths[i], materials[i].Id, startId);
                context.MassAttenuationFactors.AddRange(attenuationFactors);
                startId += attenuationFactors.Count();

                Console.WriteLine($"\tWrite Taylor 2-EXP factors for {materials[i].Name}");
                var taylorCoefficients = MaterialsParser.ReadTaylorCoefficients(paths[i], materials[i].Id, startId);
                context.Taylor2ExpFactors.AddRange(taylorCoefficients);
                startId += taylorCoefficients.Count();

                Console.WriteLine($"\tWrite GP factors for {materials[i].Name}");
                var geometricProgressionCoefficients = MaterialsParser.ReadGeometricProgressionCoefficients(paths[i], materials[i].Id, startId);
                context.GeometricProgressionFactors.AddRange(geometricProgressionCoefficients);
                startId += geometricProgressionCoefficients.Count();
            }

            context.SaveChanges();
        }

        public void FillDatabaseWithRadionuclidesData(string radionuclidesDataFolder)
        {
            var paths = RadionuclidesParser.GetRadionuclidesFilepaths(radionuclidesDataFolder);

            var radionuclides = RadionuclidesParser.ReadRadionuclidesInfo(paths);
            Console.WriteLine($"\tWrite radionuclides data. Found {radionuclides.Count} radionuclides");
            context.Radionuclides.AddRange(radionuclides);

            int startId = 1;
            for (var i = 0; i < radionuclides.Count; i++)
            {
                Console.WriteLine($"\tWrite energy-intensity data for {radionuclides[i].Name}");
                var energyIntensityData = RadionuclidesParser.ReadEnergyIntensityData(paths[i], radionuclides[i].Id, startId);
                context.RadionuclideEnergyIntensityData.AddRange(energyIntensityData);
                startId += energyIntensityData.Count();
            }

            context.SaveChanges();
        }
    }
}
