using Avalonia.Controls;
using System.Reflection;

namespace BSP.Linux.Main.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        this.Title = $"BSP v{Assembly.GetExecutingAssembly().GetName().Version} [DEVELOPER MODE]";
#else
        this.Title = $"BSP v{Assembly.GetExecutingAssembly().GetName().Version}";
#endif
    }
}