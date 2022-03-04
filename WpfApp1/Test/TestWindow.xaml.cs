using BSP.Source.XAML_Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BSP.Test
{
	/// <summary>
	/// Логика взаимодействия для TestWindow.xaml
	/// </summary>
	public partial class TestWindow : Window
	{

		public TestWindow()
		{
			InitializeComponent();
			
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			UpdaterDownloadWindow wnd = new UpdaterDownloadWindow(ApplicationUpdater.verFileUrl);
			wnd.Show();
		}


	}
}
