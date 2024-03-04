using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.ViewModels.InterpolatedDataViewer
{
    public class InterpolationViewerVM: BaseViewModel
    {
        public InterpolationViewerVM(float[] bremsstrahlungEnergies, int selectedEnvironmentMaterialId, int[] selectedMaterialsIds, Type selectedDoseFactorType, Type selectedBuildupType, MaterialsService materialsService, BuildupService buildupService)
        {
            this.energies = bremsstrahlungEnergies;
            this.selectedBuildupType = selectedBuildupType;
            this.selectedDoseFactorType = selectedDoseFactorType;

            this.materialsService = materialsService;
            this.buildupService = buildupService;

            ChoosedMaterials = materialsService.GetMaterialsById(selectedMaterialsIds);

            SelectedMaterial = ChoosedMaterials.FirstOrDefault();
        }

        private readonly MaterialsService materialsService;
        private readonly BuildupService buildupService;
        private readonly Type selectedDoseFactorType;
        private readonly Type selectedBuildupType;
        

        private readonly float[] energies;

        private MaterialDto? selectedMaterial;
        private InterpolatedParameterType selectedParameterType;

        public MaterialDto? SelectedMaterial { get => selectedMaterial; set { selectedMaterial = value; OnChanged(); UpdateView(); } }
        public InterpolatedParameterType SelectedParameterType { get => selectedParameterType; set { selectedParameterType = value; OnChanged(); } }

        public List<MaterialDto> ChoosedMaterials { get; }


        public void UpdateView()
        {
            switch (selectedParameterType)
            {
                case InterpolatedParameterType.AbsorptionFactors:
                    break;
                case InterpolatedParameterType.AttenuationFactors:
                    break;
                case InterpolatedParameterType.BuildupFactors:
                    break;
                case InterpolatedParameterType.DoseConversionFactors:
                    break;

            }
        }

        private void ClearData()
        {

        }

        private void PlotData(float[] x1, double[] y1, float[] x2, float[] y2)
        {
            

        }
    }
}
