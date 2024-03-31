using BSP.Common;

namespace BSP.ViewModels.RadionuclidesViewer
{
    public class TableRadionuclideVM : BaseViewModel
    {
        public int Id;
        public string Name { get; set; }
        public double HalfLive { get; set; }
        public string HalfLiveUnits { get; set; }

        public string FullHalfLive => string.Format("{0:G2} {1}",HalfLive, HalfLiveUnits);
    }
}
