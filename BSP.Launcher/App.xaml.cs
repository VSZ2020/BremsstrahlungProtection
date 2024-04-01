using BSP.Updater;
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

            var updater = new ApplicationUpdater();
            updater.CheckApplicationUpdate();
        }
    }

}
