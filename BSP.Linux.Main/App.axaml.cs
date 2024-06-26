using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BSP.BL.Extensions;
using BSP.Linux.Main.ViewModels;
using BSP.Linux.Main.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BSP.Linux.Main;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static IServiceProvider Provider { get; private set; }

    public override void OnFrameworkInitializationCompleted()
    {
        var servicesCollection = new ServiceCollection();
        servicesCollection.AddServices();
        servicesCollection.AddTransient<MainWindowViewModel>();

        Provider = servicesCollection.BuildServiceProvider();
        var mainWindowVM = Provider.GetRequiredService<MainWindowViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowVM,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}