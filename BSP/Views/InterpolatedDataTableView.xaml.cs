using BSP.ViewModels.TableViewer;
using System.Windows;

namespace BSP.Views
{
    /// <summary>
    /// Логика взаимодействия для InterpolatedDataTableView.xaml
    /// </summary>
    public partial class InterpolatedDataTableView : Window
    {
        private InterpolatedDataTableViewerVM vm;

        public InterpolatedDataTableView(double[] tableX, double[] tableY, double[] X, double[] Y, string title)
        {
            vm = new InterpolatedDataTableViewerVM(tableX, tableY, X, Y);
            DataContext = vm;
            InitializeComponent();

            this.Title = title;
        }
    }
}
