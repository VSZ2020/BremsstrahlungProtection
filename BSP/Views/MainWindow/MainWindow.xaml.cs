using BSP.ViewModels;
using System.Reflection;
using System.Windows;

namespace BSP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;

        public MainWindow()
        {
            vm = App.GetService<MainWindowViewModel>();
            DataContext = vm;

            InitializeComponent();

#if DEBUG
            this.Title += $"BSP v.{Assembly.GetExecutingAssembly().GetName().Version} [DEVELOPER MODE]";
#else
            this.Title += $"BSP v.{Assembly.GetExecutingAssembly().GetName().Version}";
#endif
        }
    }
}
