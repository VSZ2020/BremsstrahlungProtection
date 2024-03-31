using BSP.Common;

namespace BSP.ViewModels.RadionuclidesViewer
{
    public class RadionuclideEnergyIntensityVM: BaseViewModel
    {
        private bool _isMajorLine = false;
        public bool IsMajorLine { get => _isMajorLine; set { _isMajorLine = value; OnChanged(); } }

        public double EndpointEnergy { get; set; }
        public double AverageEnergy { get; set; }
        public double Intensity { get; set; }
    }
}
