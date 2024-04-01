using BSP.BL.Services;
using BSP.Geometries.SDK;
using System.Numerics;

namespace BSP.BL.Calculation
{
    public class InputDataBuilder
    {
        public InputDataBuilder(MaterialsService materialsService, BuildupService buildupService)
        {
            this.materialsService = materialsService;
            this.buildupService = buildupService;

            inputClass = new InputData();
        }

        private readonly MaterialsService materialsService;
        private readonly BuildupService buildupService;

        private InputData inputClass;

        public InputDataBuilder WithDimensions(float[] dims, int[] discreteness)
        {
            inputClass.Dimensions = dims;
            inputClass.Discreteness = discreteness;
            return this;
        }

        public InputDataBuilder WithEnergies(double[] energies)
        {
            inputClass.Energies = energies;
            return this;
        }

        public InputDataBuilder WithBremsstrahlungEnergyFluxes(double[] fluxes)
        {
            inputClass.PhotonsFluxes = fluxes;
            return this;
        }

        public InputDataBuilder WithEnvironmentAbsorptionFactors(double[] energies, int environmentMaterialId)
        {
            inputClass.massEnvironmentAbsorptionFactors = materialsService.GetMassAbsorptionFactors(environmentMaterialId, energies);
            return this;
        }

        public InputDataBuilder WithAttenuationFactors(int sourceMaterialId, int[] shieldMaterialsIds, double[] energies)
        {
            var massAttenuationFactors = materialsService.GetMassAttenuationFactors(CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            inputClass.massAttenuationFactors = massAttenuationFactors;
            return this;
        }

        public InputDataBuilder WithBuildup(Type? homogeneousBuildup, Type? heterogeneousBuildup, int sourceMaterialId, int[] shieldMaterialsIds, double[] energies)
        {
            if (homogeneousBuildup != null && heterogeneousBuildup != null)
            {
                inputClass.BuildupProcessor = BuildupService.GetHeterogeneousBuildupInstance(heterogeneousBuildup, homogeneousBuildup);
                inputClass.BuildupFactors = buildupService.GetInterpolatedBuildupFactors(homogeneousBuildup, CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            }
            return this;
        }

        public InputDataBuilder WithShieldLayers(List<ShieldLayer> layers)
        {
            inputClass.Layers = layers;
            return this;
        }

        public InputDataBuilder WithCalculationPoint(Vector3 vector)
        {
            inputClass.CalculationPoint = vector;
            return this;
        }

        public InputDataBuilder WithCalculationPoint(float X, float Y, float Z)
        {
            inputClass.CalculationPoint = new Vector3(X, Y, Z);
            return this;
        }

        public InputDataBuilder WithCancellationToken(CancellationToken token)
        {
            inputClass.CancellationToken = token;
            return this;
        }


        public InputDataBuilder WithSourceDensity(float density)
        {
            inputClass.SourceDensity = density;
            return this;
        }

        public InputDataBuilder WithSourceActivity(double activity)
        {
            inputClass.SourceActivity = activity;
            return this;
        }

        public InputDataBuilder WithProgress(IProgress<double> prg)
        {
            inputClass.Progress = prg;
            return this;
        }

        public InputDataBuilder WithSelfabsorption(bool isSelfAbsorptionAllowed)
        {
            inputClass.IsSelfAbsorptionAllowed = isSelfAbsorptionAllowed;
            return this;
        }

        public InputData Build()
        {
            return new InputData()
            {
                Dimensions = inputClass.Dimensions,
                Discreteness = inputClass.Discreteness,
                Energies = inputClass.Energies,
                massEnvironmentAbsorptionFactors = inputClass.massEnvironmentAbsorptionFactors,
                massAttenuationFactors = inputClass.massAttenuationFactors,
                PhotonsFluxes = inputClass.PhotonsFluxes,
                BuildupFactors = inputClass.BuildupFactors,
                BuildupProcessor = inputClass.BuildupProcessor,
                CalculationPoint = inputClass.CalculationPoint,
                CancellationToken = inputClass.CancellationToken,
                SourceDensity = inputClass.SourceDensity,
                SourceActivity = inputClass.SourceActivity,
                Layers = inputClass.Layers ?? new List<ShieldLayer>(),
                Progress = inputClass.Progress,
                IsSelfAbsorptionAllowed = inputClass.IsSelfAbsorptionAllowed
            };
        }

        private int[] CombineIds(int sourceId, int[] layersIds)
        {
            var ids = new int[layersIds.Length + 1];
            ids[0] = sourceId;
            for (var i = 0; i < layersIds.Length; i++)
                ids[i + 1] = layersIds[i];
            return ids;
        }
    }
}
