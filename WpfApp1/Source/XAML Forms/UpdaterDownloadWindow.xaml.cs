using System;
using System.Net;
using System.Windows;

namespace BSP.Source.XAML_Forms
{
	/// <summary>
	/// Логика взаимодействия для UpdaterDownloadWindow.xaml
	/// </summary>
	public partial class UpdaterDownloadWindow : Window
	{
		const string TEMP_FILENAME = "temp_BSP.exe";
		const string APP_NAME = "BSP.exe";

		public UpdaterDownloadWindow(string downloadLink)
		{
			InitializeComponent();
			DownloadFromServer(downloadLink);
		}

		public void DownloadFromServer(string link)
		{
			pbUpdaterProgress.Maximum = 100;
			pbUpdaterProgress.Value = 0;

			var client = new WebClient();
			client.DownloadFileCompleted += Client_DownloadFileCompleted;
			client.DownloadProgressChanged += Client_DownloadProgressChanged;
			client.DownloadFileAsync(new Uri(link), TEMP_FILENAME);
		}

		private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			//this.Title = e.BytesReceived.ToString() + "/" + e.TotalBytesToReceive.ToString();
		}

		private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			try
			{
				pbUpdaterProgress.Value = pbUpdaterProgress.Maximum;

				//Send to updater old and new filename
				System.Diagnostics.Process.Start("updater.exe", APP_NAME + " " + TEMP_FILENAME);

				//Меняем флаг просмотра Changelog
				MainWindow.IsChangelogShowed = false;
				MainWindow.SaveApplicationPreferences();

				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
			catch (Exception ex) 
			{ 
				// TODO: Save exception to log
				MessageBox.Show(
					(string)App.Current.Resources["updaterWindow_msgError"], 
					(string)App.Current.Resources["msgError_Title"], 
					MessageBoxButton.OK, MessageBoxImage.Error);
				this.Close();
			}
		}
	}
}
