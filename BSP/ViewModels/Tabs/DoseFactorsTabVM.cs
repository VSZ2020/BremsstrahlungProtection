using BSP.BL.Services;
using BSP.Common;

namespace BSP.ViewModels.Tabs
{
    /// <summary>
    /// Модель представления для вкладки с дозовыми коэффициентами и их зависимостями
    /// </summary>
    public class DoseFactorsTabVM : BaseViewModel
    {
        public DoseFactorsTabVM(DoseFactorsService dfService)
        {
            AvailableDoseFactors = new List<DoseFactorVM>(DoseFactorVM.Load());
            AvailableExposureGeometries = new List<ExposureGeometryVM>(ExposureGeometryVM.Load(dfService));
            AvailableOrgansAndTissues = new List<OrganTissueVM>(OrganTissueVM.Load(dfService));

            SelectedDoseFactorType = AvailableDoseFactors.FirstOrDefault();
            SelectedExposureGeometry = AvailableExposureGeometries.LastOrDefault();
            SelectedOrganTissue = AvailableOrgansAndTissues.FirstOrDefault();

            SetOptionalBoxesState(_selectedDoseFactorType);
        }

        private DoseFactorVM _selectedDoseFactorType;
        private ExposureGeometryVM _selectedExposureGeometry;
        private OrganTissueVM _selectedOrganTissue;
        private bool isExposureGeometryBoxEnabled = true;
        private bool isOrganTissueBoxEnabled = true;

        public List<DoseFactorVM> AvailableDoseFactors { get; private set; }
        public List<ExposureGeometryVM> AvailableExposureGeometries { get; private set; }
        public List<OrganTissueVM> AvailableOrgansAndTissues { get; private set; }

        public DoseFactorVM SelectedDoseFactorType { get => _selectedDoseFactorType; set { _selectedDoseFactorType = value; OnChanged(); SetOptionalBoxesState(_selectedDoseFactorType); } }
        public ExposureGeometryVM SelectedExposureGeometry { get => _selectedExposureGeometry; set { _selectedExposureGeometry = value; OnChanged(); } }
        public OrganTissueVM SelectedOrganTissue { get => _selectedOrganTissue; set { _selectedOrganTissue = value; OnChanged(); } }

        public bool IsExposureGeometryBoxEnabled { get => isExposureGeometryBoxEnabled; set { isExposureGeometryBoxEnabled = value; OnChanged(); } }
        public bool IsOrganTissueBoxEnabled { get => isOrganTissueBoxEnabled; set { isOrganTissueBoxEnabled = value; OnChanged(); } }

        private void SetOptionalBoxesState(DoseFactorVM selectedDoseFactorType)
        {
            (var eg_state, var ot_state) = DoseFactorsService.GetOptionalBoxesState(selectedDoseFactorType.DoseFactorType);
            IsExposureGeometryBoxEnabled = eg_state;
            IsOrganTissueBoxEnabled = ot_state;
        }
    }
}
