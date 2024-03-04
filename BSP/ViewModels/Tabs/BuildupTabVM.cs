using BSP.Common;
using System.Collections.Generic;
using System.Linq;

namespace BSP.ViewModels.Tabs
{
    public class BuildupTabVM : BaseViewModel
    {
        public BuildupTabVM()
        {
            AvailableBuildups = new List<BuildupVM>(BuildupVM.Load());
            AvailableHeterogeneousBuildups = new(HeterogeneousBuildupVM.Load());
            SelectedBuildup = AvailableBuildups.FirstOrDefault();
            SelectedComplexBuildup = AvailableHeterogeneousBuildups.FirstOrDefault();
        }

        private bool isIncludeBuildup = true;
        private BuildupVM _selectedBuildup;
        private HeterogeneousBuildupVM _selectedHeterogeneousBuildup;

        public BuildupVM SelectedBuildup { get => _selectedBuildup; set { _selectedBuildup = value; OnChanged(); } }
        public HeterogeneousBuildupVM SelectedComplexBuildup { get => _selectedHeterogeneousBuildup; set { _selectedHeterogeneousBuildup = value; OnChanged(); } }

        public List<BuildupVM> AvailableBuildups { get; }
        public List<HeterogeneousBuildupVM> AvailableHeterogeneousBuildups { get; }

        public bool IsIncludeBuildup { get => isIncludeBuildup; set { isIncludeBuildup = value; OnChanged(); } }
    }
}
