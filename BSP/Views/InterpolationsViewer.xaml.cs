using BSP.BL.Services;
using BSP.ViewModels.InterpolatedDataViewer;
using System.Windows;

namespace BSP.Views
{
    /// <summary>
    /// Логика взаимодействия для InterpolationsViewer.xaml
    /// </summary>
    public partial class InterpolationsViewer : Window
    {
        private InterpolationViewerVM vm;

        public InterpolationsViewer(double[] bremsstrahlungEnergies, int selectedEnvironmentMaterialId, int[] selectedMaterialsIds, Type selectedBuildupType, MaterialsService materialsService, BuildupService buildupService, DoseFactorsService doseFactorsService, Type selectedDoseFactorType, int exposureGeometryId, int OrganTissueId)
        {
            this.vm = new InterpolationViewerVM(
                bremsstrahlungEnergies, 
                selectedEnvironmentMaterialId, 
                selectedMaterialsIds, 
                selectedBuildupType, 
                materialsService, 
                buildupService, 
                doseFactorsService, 
                selectedDoseFactorType, 
                exposureGeometryId, 
                OrganTissueId);
            DataContext = vm;
            InitializeComponent();
        }
    }
}
