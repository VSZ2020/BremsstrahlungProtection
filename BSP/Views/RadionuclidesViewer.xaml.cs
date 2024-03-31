using BSP.BL.Services;
using BSP.ViewModels.RadionuclidesViewer;
using System.Windows;

namespace BSP.Views
{
    /// <summary>
    /// Логика взаимодействия для RadionuclidesViewer.xaml
    /// </summary>
    public partial class RadionuclidesViewer : Window
    {
        public RadionuclidesViewer(RadionuclidesService service)
        {
            vm = new RadionuclidesViewerVM(service);
            this.DataContext = vm;
            InitializeComponent();
        }

        private readonly RadionuclidesViewerVM vm;
    }
}
