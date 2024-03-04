using BSP.Source.XAML_Forms;
using BSP.Updater;
using System.Configuration;
using System.Data;
using System.Windows;

namespace BSP.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppSplashScreen screen = new AppSplashScreen();
            this.MainWindow = screen;
            screen.Show();
            var updater = new ApplicationUpdater();
            updater.CheckApplicationUpdate();
            screen.Close();
        }
    }

}
