using BSP.BL.Buildups.Common;
using BSP.BL.Materials;
using BSP.BL.Services;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BSP.BL.Calculation
{
    public class InputDataBuilder
    {
        public InputDataBuilder(RadionuclidesService radionuclidesService, MaterialsService materialsService, BuildupService buildupService, DoseFactorsService dcfService)
        {
            this.materialsService = materialsService;
            this.radionuclidesService = radionuclidesService;
            this.buildupService = buildupService;
            this.doseFactorsService = dcfService;
        }

        private readonly RadionuclidesService radionuclidesService;
        private readonly MaterialsService materialsService;
        private readonly BuildupService buildupService;
        private readonly DoseFactorsService doseFactorsService;

        public float[] massEnvironmentAbsorptionFactors;
        public float[][] massAttenuationFactors;
        public float[][][] BuildupFactors;
        public float[] DoseConversionFactors;

        public List<ShieldLayer> Layers = new List<ShieldLayer>();

        public double SourceActivity = 0;

        public float SourceDensity = 0;

        /// <summary>
        /// Флаг учета самопоглощения в материале источника
        /// </summary>
        public bool IsSelfAbsorptionAllowed = false;

        /// <summary>
        /// Расстояние от точечного источника до точки регистрации излучения
        /// </summary>
        public double CalculationDistance = 1;

        /// <summary>
        /// Рассчитанные выходы энергий тормозного излучения [МэВ/распад]
        /// </summary>
        public double[] BremsstrahlungYields;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public CancellationToken CancellationToken;

        public IProgress<int> Progress;


        public InputDataBuilder WithBremsstrahlungYields(int[] selectedRadionuclidesIds, float sourceZeff)
        {
            (_, var energies, var yields) = radionuclidesService.GetEnergyIntensityDataArrays(selectedRadionuclidesIds);
            var bremsstrahlungEnergyYields = Bremsstrahlung.GetBremsstrahlungEnergyYields(energies, yields, sourceZeff);
            this.BremsstrahlungYields = bremsstrahlungEnergyYields;
            return this;
        }

        public InputDataBuilder WithBremsstrahlungYields(double[] yields)
        {
            this.BremsstrahlungYields = yields;
            return this;
        }

        public InputDataBuilder WithEnvironmentAbsorptionFactors(float[] energies, int environmentMaterialId)
        {
            this.massEnvironmentAbsorptionFactors = materialsService.GetAbsorptionFactors(environmentMaterialId, energies);
            return this;
        }

        public InputDataBuilder WithAttenuationFactors(int sourceMaterialId, int[] shieldMaterialsIds, float[] energies)
        {

            this.massAttenuationFactors = materialsService.GetMassAttenuationFactors(CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            return this;
        }

        public InputDataBuilder WithBuildup(Type homogeneousBuildup, Type? heterogeneousBuildup, int sourceMaterialId, int[] shieldMaterialsIds, float[] energies)
        {
            if (homogeneousBuildup != null && heterogeneousBuildup != null)
            {
                this.BuildupProcessor = BuildupService.GetHeterogeneousBuildupInstance(heterogeneousBuildup, homogeneousBuildup);
                this.BuildupFactors = buildupService.GetInterpolatedBuildupFactors(homogeneousBuildup, CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            }
            return this;
        }

        public InputDataBuilder WithShieldLayers(List<ShieldLayer> layers)
        {
            this.Layers = layers;
            return this;
        }

        public InputDataBuilder WithCalculationPoint(double distance)
        {
            this.CalculationDistance = distance;
            return this;
        }

        public InputDataBuilder WithCancellationToken(CancellationToken token)
        {
            this.CancellationToken = token;
            return this;
        }

        public InputDataBuilder WithDoseConversionFactors(Type dcfType, float[] energies, int geometryId, int organId)
        {
            this.DoseConversionFactors = doseFactorsService.GetDoseConversionFactors(dcfType, energies, geometryId, organId);
            return this;
        }

        public InputDataBuilder WithSourceActivityAndDensity(double activity, float density)
        {
            this.SourceActivity = activity;
            this.SourceDensity = density;
            return this;
        }

        public InputDataBuilder WithProgress(IProgress<int> prg)
        {
            this.Progress = prg;
            return this;
        }

        public InputData Build()
        {
            return new InputData()
            {
                massEnvironmentAbsorptionFactors = this.massEnvironmentAbsorptionFactors,
                massAttenuationFactors = this.massAttenuationFactors,
                DoseConversionFactors = this.DoseConversionFactors ?? new float[massAttenuationFactors.Length],
                BremsstrahlungYields = this.BremsstrahlungYields,
                BuildupFactors = this.BuildupFactors,
                BuildupProcessor = this.BuildupProcessor,
                CalculationDistance = this.CalculationDistance,
                CancellationToken = this.CancellationToken,
                SourceDensity = this.SourceDensity,
                SourceActivity = this.SourceActivity,
                Layers = this.Layers ?? new List<ShieldLayer>(),
                Progress = this.Progress,
            };
        }

        private int[] CombineIds(int sourceId, int[] layersIds)
        {
            var ids = new int[layersIds.Length + 1];
            ids[0] = sourceId;
            for (var i = 1; i < layersIds.Length + 1; i++)
                ids[i] = layersIds[i];
            return ids;
        }
    }
}
