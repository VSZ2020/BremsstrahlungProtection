using BSP.BL.Buildups.Common;
using BSP.BL.Materials;
using BSP.BL.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BSP.BL.Calculation
{
    public class InputDataBuilder
    {
        public InputDataBuilder(RadionuclidesService radionuclidesService, MaterialsService materialsService, BuildupService buildupService, DoseFactorsService doseFactorsService)
        {
            this.materialsService = materialsService;
            this.radionuclidesService = radionuclidesService;
            this.buildupService = buildupService;
            this.doseFactorsService = doseFactorsService;
        }

        private readonly RadionuclidesService radionuclidesService;
        private readonly MaterialsService materialsService;
        private readonly BuildupService buildupService;
        private readonly DoseFactorsService doseFactorsService;

        public double[] massEnvironmentAbsorptionFactors;
        public double[][] massAttenuationFactors;
        public double[][][] BuildupFactors;

        public List<ShieldLayer> Layers = new List<ShieldLayer>();

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
        public double[] BremsstrahlungFluxes;

        /// <summary>
        /// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        /// </summary>
        public BaseHeterogeneousBuildup BuildupProcessor;

        public CancellationToken CancellationToken;

        public IProgress<int> Progress;


        public InputDataBuilder WithBremsstrahlungEnergyFluxes(int[] selectedRadionuclidesIds, float sourceZeff, double activity)
        {
            (_, var energies, var yields) = radionuclidesService.GetEnergyIntensityDataArrays(selectedRadionuclidesIds);
            var bremsstrahlungEnergyYields = Bremsstrahlung.GetBremsstrahlungEnergyYields(energies, yields, sourceZeff);
            
            this.BremsstrahlungFluxes = Bremsstrahlung.GetBremsstrahlungFluxOfEnergy(bremsstrahlungEnergyYields, activity);
            return this;
        }

        public InputDataBuilder WithBremsstrahlungEnergyFluxes(double[] fluxes)
        {
            this.BremsstrahlungFluxes = fluxes;
            return this;
        }

        public InputDataBuilder WithEnvironmentAbsorptionFactors(double[] energies, int environmentMaterialId)
        {
            this.massEnvironmentAbsorptionFactors = materialsService.GetAbsorptionFactors(environmentMaterialId, energies);
            return this;
        }

        public InputDataBuilder WithAttenuationFactors(int sourceMaterialId, int[] shieldMaterialsIds, double[] energies)
        {

            this.massAttenuationFactors = materialsService.GetMassAttenuationFactors(CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            return this;
        }

        public InputDataBuilder WithBuildup(Type homogeneousBuildup, Type? heterogeneousBuildup, int sourceMaterialId, int[] shieldMaterialsIds, double[] energies)
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


        public InputDataBuilder WithSourceDensity(float density)
        {
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
                BremsstrahlungFlux = this.BremsstrahlungFluxes,
                BuildupFactors = this.BuildupFactors,
                BuildupProcessor = this.BuildupProcessor,
                CalculationDistance = this.CalculationDistance,
                CancellationToken = this.CancellationToken,
                SourceDensity = this.SourceDensity,
                Layers = this.Layers ?? new List<ShieldLayer>(),
                Progress = this.Progress,
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

        public string ExportToString(double[] energies, int environmentMaterialId, int sourceMaterialId, int[] selectedMaterialsIds, Type? homogeneousBuildupType = null, Type? doseFactorType = null, int exposureGeometryId = 1, int organId = 1,  bool exportAllowed = false)
        {
            if (exportAllowed)
            {
                char colDelimeter = '\t';

                string envMaterialName = materialsService.GetMaterialById(environmentMaterialId).Name;
                int[] materialsIds = CombineIds(sourceMaterialId, selectedMaterialsIds);
                var materialsNames = materialsService.GetMaterialsById(materialsIds).Select(m => m.Name).ToArray();
                var buildupCoefficientsNames = BuildupService.GetBuildupCoefficientsNames(homogeneousBuildupType);

                StringBuilder builder = new();
                builder.AppendLine("\n===== Interpolated Input Data ====="); 
                builder.AppendLine("Designations: BEF - Bremsstrahlung energy flux; MEAF - mass environments absorption factor; MAF - mass attenuation factor.");

                //Заголовок данных по тормозному излучению
                builder.AppendLine(string.Format("{0}{1}{2}", "Energy(MeV)", colDelimeter, "BEF(MeV/s)"));
                //Записываем данные по тормозному излучению
                for (var i = 0; i < energies.Length; i++)
                {
                    builder.AppendLine(string.Format("{0:e3}{1}{2:e3}", energies[i], colDelimeter, this.BremsstrahlungFluxes[i]));
                }


                builder.AppendLine();
                //Записываем заголовок для коэффициента поглощения в среде
                builder.Append(string.Format("{0}{1}{2}-{3}{4}", "Energy(MeV)", colDelimeter, "MEAF(cm2/g)", envMaterialName, colDelimeter));

                //Записываем заголовки данных по коэффициентам ослабления для каждого материала
                for (var j = 0; j < materialsNames.Length; j++)
                {
                    builder.Append(string.Format("MAF(cm2/g)-{0}{1}", materialsNames[j], colDelimeter));
                }
                builder.Append("\n");

                //Записываем данные по коэффициентам ослабления для каждого материала
                for (var i = 0; i < energies.Length; i++)
                {
                    //Записываем значение для коэффициента поглощения в среде
                    builder.Append(string.Format("{0:e3}{1}{2:e3}{3}", energies[i], colDelimeter, this.massEnvironmentAbsorptionFactors[i], colDelimeter));

                    //Записываем значения коэффициентов ослабления для каждого материала
                    for (var j = 0; j < materialsNames.Length; j++)
                        builder.Append(string.Format("{0:e3}{1}", this.massAttenuationFactors[i][j], colDelimeter));

                    builder.Append("\n");
                }


                if (this.BuildupProcessor != null)
                {
                    builder.AppendLine();

                    builder.Append(string.Format("{0}{1}", "Energy(MeV)", colDelimeter));
                    //Записываем заголовки для коэффициентов формулы фактора накопления для каждого материала
                    for (var j = 0; j < materialsNames.Length; j++)
                        for (var k = 0; k < buildupCoefficientsNames.Length; k++)
                            builder.Append(string.Format("{0}({1}){2}", buildupCoefficientsNames[k], materialsNames[j], colDelimeter));
                    builder.Append("\n");

                    //Записываем значения коэффициентов формулы расчета фактора накопления для каждой энергии
                    for (var i = 0; i < energies.Length; i++)
                    {
                        builder.Append(string.Format("{0:e3}{1}", energies[i], colDelimeter));

                        //Записываем значения коэффициентов расчета фактора накопления для каждого материала
                        for (var j = 0; j < materialsNames.Length; j++)
                            for (var k = 0; k < buildupCoefficientsNames.Length; k++)
                                builder.Append(string.Format("{0:e3}{1}", this.BuildupFactors[i][j][k], colDelimeter));

                        builder.Append("\n");
                    }
                }

                if (doseFactorType != null)
                {
                    builder.AppendLine();
                    var doseFactors = doseFactorsService.GetDoseConversionFactors(doseFactorType, energies, exposureGeometryId, organId);
                    var doseFactorName = DoseFactorsService.DoseFactors[doseFactorType];
                    var units = DoseFactorsService.GetDoseConversionFactorUnits(doseFactorType);

                    builder.Append(string.Format("{0}{1}{2}\n", "Energy(MeV)", colDelimeter, $"{doseFactorName} ({units})"));
                    for (var i = 0; i < energies.Length; i++)
                    {
                        builder.AppendLine(string.Format("{0:e3}{1}{2:e3}{3}", energies[i], colDelimeter, doseFactors[i], colDelimeter));
                    }
                }

                builder.AppendLine("\n===== End of Interpolated Input Data =====");
                builder.AppendLine();

                return builder.ToString();
            }
            return "";
        }
    }
}
