using System;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;

namespace BSP.Source.XAML_Forms
{
	/// <summary>
	/// Логика взаимодействия для AppSplashScreen.xaml
	/// </summary>
	public partial class AppSplashScreen : Window
	{
		public string StatusText
		{
			get
			{
				return statusLabel.Content.ToString();
			}
			set
			{
				statusLabel.Content = value;
				//statusLabel.Dispatcher.InvokeShutdown();
			}
		}

		public IProgress<string> StatusChanged;

		public AppSplashScreen()
		{
			InitializeComponent();
			StatusChanged = new Progress<string>((msg) =>
			{
				statusLabel.Content = msg;
			});
		}
	}
}
