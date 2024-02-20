using BSP.Source.Calculation.CalcDirections;
using BSP.Source.XAML_Forms;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace BSP
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public System.Globalization.NumberFormatInfo culture = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
		private CancellationTokenSource cancelSource;
		public CancellationToken cancelToken;
		private IProgress<int> progress;

		public delegate void SendTextToOutput(OutputValue result);
		public event SendTextToOutput OnDoseResultAppear;

		public InputData data;
		public double bufferSourceSizeX { get; set; } = 20.0;
		public double bufferSourceSizeY { get; set; } = 20.0;
		public double bufferSourceSizeZ { get; set; } = 20.0;

		public enum MessageLevel
		{
			Info, Error, Service
		}

		public MainWindow()
		{
			AppSplashScreen screen = new AppSplashScreen();
			screen.Show();

			//CheckUpdate
			//screen.StatusChanged.Report((string)Application.Current.Resources["label_CheckUpdates"] + " ...");

			var updater = new ApplicationUpdater();
			updater.CheckApplicationUpdate();

			data = new InputData();

			InitializeComponent();
			InitMainMenu(ref MainMenu);
			App.LanguageChanged += App_LanguageChanged;

			//Initialization of components
			InitializeDatabases(ref screen);

			//screen.StatusText = (string)Application.Current.Resources["label_InitUserInterface"];

			InitializeRadiatonSourceTab();
			InitializePopupMenu();
			InitializeShieldListBox();
			InitializeOutputTab();
			InitializeStatusBar();
			InitializeOutputBox();

			OnDoseResultAppear += MainWindow_OnDoseResultAppear;

			UnblockInterface(true);

			screen.Close();
#if DEBUG
			this.Title += $" v{Assembly.GetExecutingAssembly().GetName().Version.ToString()} [DEBUG MODE]";
#endif
			this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
		}

        private void MainWindow_Closed(object sender, EventArgs e)
        {
			SaveApplicationPreferences();
		}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (File.Exists("BSP.config"))
            {
				var buf = File.ReadAllLines("BSP.config");
				bool.TryParse(buf[0], out IsChangelogShowed);
            }
			Thread th = new Thread(() =>
			{
				Thread.Sleep(1000);
				InitializeChangelog();
			});
			th.Start();
        }

		public static void SaveApplicationPreferences()
        {
			//Saving application parameters
			try
			{
				File.WriteAllLines("BSP.config", new string[] { IsChangelogShowed.ToString() });
			}
			catch (Exception ex)
			{
				Debugger.Log(0, "Errors", ex.Message);
			}
		}

		private void App_LanguageChanged(object sender, EventArgs e)
		{
			CultureInfo info = App.Language;
			foreach (MenuItem item in MainMenu.Items)
			{
				var culture = item.Tag as CultureInfo;
				item.IsChecked = culture != null && culture.Equals(info);
			}

			MessageBox.Show((string)Application.Current.Resources["title_RestartApplication"],
				(string)Application.Current.Resources["msgWarning_Title"],
				MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			CalculateDosePower();
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			if (btnStop.IsEnabled)
				cancelSource?.Cancel();
		}

		public async void CalculateDosePower()
		{
			cancelSource = new CancellationTokenSource();

			progressBar.Value = 0;
			cancelToken = cancelSource.Token;

			if (ReadInputDataFields(ref data))
			{
				//Block interface buttons and inputs
				UnblockInterface(false);

				data.token = cancelToken;
				data.progressUpdater = progress;

				statusbarLabel.Text = (string)Application.Current.Resources["msgStatus_Calc"];
				Calculation currentCalc = new Calculation();
				OutputValue result = await currentCalc.StartAsync(data);

				if (!cancelToken.IsCancellationRequested)
				{
					OnDoseResultAppear?.Invoke(result);

					statusbarLabel.Text = (string)Application.Current.Resources["msgStatus_Ready"];
				}
				else
				{
					statusbarLabel.Text = (string)Application.Current.Resources["msgStatus_Cancel"];
					cancelSource.Dispose();
					cancelSource = new CancellationTokenSource();
				}

				UnblockInterface(true);
			}

		}

		private void MainWindow_OnDoseResultAppear(OutputValue result)
		{
			var selDoseType = (DoseFactor)cbOutDoseType.SelectedItem;
			var selDirection = (ADirection)cbRadiationDirection.SelectedItem;
			var selForm = (ASourceForm)cbSourceGeometry.SelectedItem;

			SendMessage(string.Format(
				"{0}. " + (string)Application.Current.Resources["tabRadsource_SourceMaterialBox"] + " - {1}, b = {2}. {3} {4} = {5:E" + CalcParams.precision + "} {6}",
				selForm.Name,
				CalcParams.Source.Substances.Name,
				CalcParams.CalculationDistance,
				$"{(string)Application.Current.Resources["DoseRate_Title"]} {selDoseType.Name}",
				selDirection.Name.ToLower(),
				result.DoseRate,
				selDoseType.DoseRateUnits
				), MessageLevel.Info);

			//Если отмечена галочка "Парциальные мощности доз", то выводим их			
			if (cbShowPartialDoses.IsChecked.Value && result.DoseRatePart != null)
			{
				int parts = result.DoseRatePart.Length;
				for (int i = 0; i < parts; i++)
					SendMessage(string.Format("#{0}:\t{1:E" + CalcParams.precision + "} {2}", i + 1, result.DoseRatePart[i], selDoseType.DoseRateUnits), MessageLevel.Info);
			}
		}

		public void SendMessage(string msg, MessageLevel level)
		{
			TextRange range = new TextRange(outputBox.Document.ContentEnd.DocumentEnd, outputBox.Document.ContentEnd.DocumentEnd);
			range.Text = msg + "\n";
			
			if (level == MessageLevel.Error)
			{
				range.ApplyPropertyValue(RichTextBox.ForegroundProperty, new SolidColorBrush(Colors.Red));
			}
			if (level == MessageLevel.Service)
			{
				range.ApplyPropertyValue(RichTextBox.ForegroundProperty, new SolidColorBrush(Colors.Blue));
			}

			outputBox.ScrollToEnd();
		}

		public void UnblockInterface(bool IsBlock)
		{
			var menuLaunch = (MenuItem)MainMenu.Items[2];
			((MenuItem)menuLaunch.Items[0]).IsEnabled = IsBlock;	//Start calc btn
			((MenuItem)menuLaunch.Items[1]).IsEnabled = !IsBlock;	//Stop calc btn

			cbIncludeScattering.IsEnabled = IsBlock;
			cbShowPartialDoses.IsEnabled = IsBlock;
			cbIsPointSource.IsEnabled = IsBlock;
			btnStart.IsEnabled = IsBlock;
			btnStop.IsEnabled = !btnStart.IsEnabled;

			tabControl.IsEnabled = IsBlock;

		}


	}
}
