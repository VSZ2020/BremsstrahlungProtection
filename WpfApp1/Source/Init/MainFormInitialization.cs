using BSP.Source.Calculation.CalcDirections;
using BSP.Source.XAML_Forms;
using BSP.Test;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace BSP
{
	public partial class MainWindow: Window
	{
		const string DATABASE_BSDATA = "data\\BS_Data.txt";
		const string DATABASE_NUCLIDES = "data\\Nuclides.mdb";
		const string DATABASE_MATERIALS = "data\\Materials.mdb";
		const string DATABASE_DOSEFACTORS = "data\\DoseFactors.mdb";

		public static bool IsChangelogShowed = false;

		public enum OperationType
        {
			Add, Edit, Delete
        }
		public enum ItemType
        {
			Nuclide, ShieldLayer
        }

		private void InitializeChangelog()
		{
			
			if (!IsChangelogShowed)
			{
				ShowChangelogWindow();
				IsChangelogShowed = true;
			}

		}
		/// <summary>
		/// Показывает окно со списком изменений
		/// </summary>
		private void ShowChangelogWindow()
		{
			
			StringBuilder msg = new StringBuilder();
			string text = (string)Application.Current.Resources["changelog_whatsNew"];
			//Формируем сообщение
			if (!text.Equals("_"))
			{
				msg.AppendLine((string)Application.Current.Resources["changelog_whatsNewTitle"] + ":");
				msg.AppendLine(" - " + text);
			}

			text = (string)Application.Current.Resources["changelog_changes"];
			if (!text.Equals("_"))
			{
				msg.AppendLine("\n" + (string)Application.Current.Resources["changelog_changesTitle"] + ":");
				msg.AppendLine(" - " + text);
			}

			text = (string)Application.Current.Resources["changelog_critical"];
			if (!text.Equals("_"))
			{
				msg.AppendLine("\n" + (string)Application.Current.Resources["changelog_criticalTitle"] + ":");
				msg.AppendLine(" - " + text);
			}

			//Показываем сообщение
			if (msg.Length > 0)
			MessageBox.Show(
				msg.ToString(),
				$"BSP v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}", 
				MessageBoxButton.OK, MessageBoxImage.None);
		}

		public void InitMainMenu(ref Menu menu)
		{
			//Заполняем раздел меню "Программа"
			var itemProgram = new MenuItem();
			var itemLanguage = new MenuItem();
			var itemExit = new MenuItem();

			itemProgram.Header = (string)menu.TryFindResource("mmProgram");
			itemLanguage.Header = (string)menu.TryFindResource("miLanguage");
			itemExit.Header = (string)menu.TryFindResource("miExit");

			itemProgram.Items.Add(itemLanguage);

			CultureInfo currLanguage = App.Language;
			foreach (var lang in App.Languages)
			{
				var langItem = new MenuItem();
				langItem.Header = lang.DisplayName;
				langItem.Tag = lang;
				langItem.IsChecked = lang.Equals(currLanguage);
				langItem.Click += LanguageChangeItem_Click;
				itemLanguage.Items.Add(langItem);
			}

			itemProgram.Items.Add(new Separator());
			itemProgram.Items.Add(itemExit);
			menu.Items.Add(itemProgram);

			//Edit menu segment
			var itemEdit = new MenuItem();
			itemEdit.Header = (string)menu.TryFindResource("mmEdit");
			//Edit subitems
			var subItemAdd = new MenuItem();
			var subItemReplace = new MenuItem();
			var subItemRemove = new MenuItem();

			subItemAdd.Header = (string)menu.TryFindResource("miEdit_Add");
			subItemReplace.Header = (string)menu.TryFindResource("miEdit_Edit");
			subItemRemove.Header = (string)menu.TryFindResource("miEdit_Remove");

			subItemAdd.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/add.png", UriKind.Relative)) };
			subItemReplace.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/edit.png", UriKind.Relative)) };
			subItemRemove.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/remove.png", UriKind.Relative)) };

			subItemAdd.Click += (object sender, RoutedEventArgs e) => 
			{
				HandleListItems(OperationType.Add, (tabControl.SelectedIndex == 0) ? ItemType.Nuclide : ItemType.ShieldLayer);
			};
			subItemReplace.Click += (object sender, RoutedEventArgs e) =>
			{
				HandleListItems(OperationType.Edit, (tabControl.SelectedIndex == 0) ? ItemType.Nuclide : ItemType.ShieldLayer);
			};
			subItemRemove.Click += (object sender, RoutedEventArgs e) =>
			{
				HandleListItems(OperationType.Delete, (tabControl.SelectedIndex == 0) ? ItemType.Nuclide : ItemType.ShieldLayer);
			};

			itemEdit.Items.Add(subItemAdd);
			itemEdit.Items.Add(subItemReplace);
			itemEdit.Items.Add(subItemRemove);
			menu.Items.Add(itemEdit);

			//Заполняем раздел меню "Запуск"
			var itemLaunch = new MenuItem();
			itemLaunch.Header = (string)menu.TryFindResource("mmLaunch");

			var itemStartCalc = new MenuItem();
			itemStartCalc.Header = (string)menu.TryFindResource("miLaunchCalc");
			itemStartCalc.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/iconPlay.png", UriKind.Relative)) };
			itemStartCalc.InputGestureText = "F5";
			itemStartCalc.Click += btnStart_Click;

			var itemStopCalc = new MenuItem();
			itemStopCalc.Header = (string)menu.TryFindResource("miStopCalc");
			itemStopCalc.Icon =  new Image() { Source = new BitmapImage(new Uri("/Resources/Images/iconStop.png", UriKind.Relative)) };
			itemStopCalc.InputGestureText = "F6";
			itemStopCalc.Click += btnStop_Click;

			//var itemLaunchInterpEditor = new MenuItem();
			//itemLaunchInterpEditor.Header = (string)menu.TryFindResource("miLaunchInterpViewer");

			itemLaunch.Items.Add(itemStartCalc);
			itemLaunch.Items.Add(itemStopCalc);
			//itemLaunch.Items.Add(new Separator());
			//itemLaunch.Items.Add(itemLaunchInterpEditor);

			menu.Items.Add(itemLaunch);

			//Fill the help section
			var itemHelpMenu = new MenuItem();
			itemHelpMenu.Header = (string)Application.Current.Resources["mmHelp"];

			var itemHelp = new MenuItem();
			var itemShowChangelog = new MenuItem();
			var itemAbout = new MenuItem();

			itemHelp.Header = (string)Application.Current.Resources["miHelp"];
			itemShowChangelog.Header = (string)Application.Current.Resources["miShowChangelog"];
			itemAbout.Header = (string)Application.Current.Resources["miAbout"];
            itemAbout.Click += ItemAbout_Click;

			itemShowChangelog.Click += (object sender, RoutedEventArgs e) =>
			{
				ShowChangelogWindow();
			};

            itemExit.Click += ItemExit_Click;

			itemHelpMenu.Items.Add(itemHelp);
			itemHelpMenu.Items.Add(itemShowChangelog);
			itemHelpMenu.Items.Add(new Separator());
			itemHelpMenu.Items.Add(itemAbout);

			menu.Items.Add(itemHelpMenu);

			//DeveloperButtons
#if DEBUG
			var itemDevOps = new MenuItem();
			itemDevOps.Header = "DevOps";
			var itemDevTestForm = new MenuItem();
			itemDevTestForm.Header = "Run TestForm";
			itemDevOps.Items.Add(itemDevTestForm);
			menu.Items.Add(itemDevOps);
			itemDevTestForm.Click += (object sender, RoutedEventArgs e) =>
			{
				TestWindow form = new TestWindow();
				form.ShowDialog();
			};
#endif

		}

        private void ItemExit_Click(object sender, RoutedEventArgs e)
        {
			this.Close();
        }

        private void ItemAbout_Click(object sender, RoutedEventArgs e)
        {
			(new About()).ShowDialog();
        }

        private void LanguageChangeItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MenuItem mItem = sender as MenuItem;
			if (mItem != null)
			{
				CultureInfo info = mItem.Tag as CultureInfo;
				if (info != null)
					App.Language = info;
			}
		}

		/// <summary>
		/// Init databases by loading mdb data
		/// </summary>
		/// <param name="statusLabel"></param>
		private void InitializeDatabases(ref AppSplashScreen splash)
		{
            try
            {
				//var status = splash.StatusText;
				//splash.StatusChanged.Report((string) Application.Current.Resources["label_LoadGroupCoefficients"]);
				Breamsstrahlung.BsFactor = ExternalDataReader.LoadBSDataFromTextFile(DATABASE_BSDATA, culture);

				//status = (string)Application.Current.Resources["label_LoadNuclides"];
				CalcParams.TableNuclides = ExternalDataReader.LoadNuclidesFromDB(DATABASE_NUCLIDES, Breamsstrahlung.Length, culture);

				//status = (string)Application.Current.Resources["label_LoadMaterials"];
				CalcParams.TableMaterials = ExternalDataReader.LoadMaterialsFromDB(DATABASE_MATERIALS);

				//status = (string)Application.Current.Resources["label_LoadDoseFactors"];
				CalcParams.TableDoseFactors = ExternalDataReader.LoadDoseFactorsFromDB(DATABASE_DOSEFACTORS, culture);
			}
			catch(Exception ex)
            {
				UnblockInterface(true);
				Debugger.Log(0, "Errors", ex.Message);
				//MessageBox.Show(ex.Message, (string)Application.Current.Resources["msgError_Title"],
				//	MessageBoxButton.OK, MessageBoxImage.Error);
				MessageBox.Show((string)Application.Current.Resources["msgError_AbsensePrimeFiles"],
					 (string)Application.Current.Resources["msgError_Title"],
					 MessageBoxButton.OK, MessageBoxImage.Error);
				Process.GetCurrentProcess().Kill();
			}
			
		}

		public void InitializeRadiatonSourceTab()
		{
			CalcParams.Source = new RadiationSource();

			InitializeSelectedNuclidesList();
			InitializeCalculationDirectionsList();
			InitializeSourceMaterialsList();
			InitializeSourceFormsList();
			InitializeSourceSizeBoxes();
		}

		public void InitializeSourceMaterialsList()
		{
			cbSourceMaterial.ItemsSource = CalcParams.TableMaterials.Collection;
			cbSourceMaterial.SelectionChanged += CbSourceMaterial_SelectionChanged;

			//Link values to CalcParams.Source [Z] and [Density] values and add them validation rules
			SetDoubleValueBindingTo(tbSourceZeff, CalcParams.Source, "Z", BindingMode.TwoWay);
			SetDoubleValueBindingTo(tbSourceDensity, CalcParams.Source, "Density", BindingMode.TwoWay);

			//Text for ERROR messages
			tbSourceZeff.Tag = (string)Application.Current.Resources["tabRadsource_SourceMaterialZeffLabel"];
			tbSourceDensity.Tag = (string)Application.Current.Resources["tabRadsource_SourceMaterialDensityLabel"];
			//Select the FIRST item in materials list
			if (cbSourceMaterial.Items.Count > 0) cbSourceMaterial.SelectedIndex = 0;
		}

		private void CbSourceMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			tbSourceZeff.Text = (cbSourceMaterial.SelectedItem as Material)?.Z.ToString() ?? "0.0";
			tbSourceDensity.Text = (cbSourceMaterial.SelectedItem as Material)?.Density.ToString() ?? "0.0";
		}

		public void InitializeSourceFormsList()
		{
			CalcParams.SourceForms = new List<ASourceForm>();
			CalcParams.SourceForms.Add(new Cylinder());
			CalcParams.SourceForms.Add(new Parallelepiped());

			cbSourceGeometry.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
			{
				if (cbSourceGeometry.SelectedIndex == 0)
				{
					tbSourceLength.IsEnabled = false;
				}
				else
				{
					tbSourceLength.IsEnabled = true;
				}
			};

			cbSourceGeometry.ItemsSource = CalcParams.SourceForms;
			if (cbSourceGeometry.Items.Count > 0) cbSourceGeometry.SelectedIndex = 0;

			InitializeSourceSizeBoxes();
		}

		private void InitializeSourceSizeBoxes()
		{
			SetDoubleValueBindingTo(tbSourceLength, this, "bufferSourceSizeX");
			SetDoubleValueBindingTo(tbSourceWidth, this, "bufferSourceSizeY");
			SetDoubleValueBindingTo(tbSourceHeight, this, "bufferSourceSizeZ");

			tbSourceLength.Tag = (string)Application.Current.Resources["tabRadsource_SourceLengthLabel"];
			tbSourceWidth.Tag = (string)Application.Current.Resources["tabRadsource_SourceWidthLabel"];
			tbSourceHeight.Tag = (string)Application.Current.Resources["tabRadsource_SourceHeightLabel"];
		}

		public void InitializeCalculationDirectionsList()
		{
			CalcParams.CalculationDirectionsList = new List<Source.Calculation.CalcDirections.ADirection>();
			CalcParams.CalculationDirectionsList.Add(new XCalcDirection());
			CalcParams.CalculationDirectionsList.Add(new YCalcDirection());
			CalcParams.CalculationDirectionsList.Add(new ZCalcDirection());

			cbRadiationDirection.ItemsSource = CalcParams.CalculationDirectionsList;
			if (cbRadiationDirection.Items.Count > 0) cbRadiationDirection.SelectedIndex = 0;
		}

		/// <summary>
		/// Инициирует список выбранных нуклидов
		/// </summary>
		public void InitializeSelectedNuclidesList()
		{
			CalcParams.Source.Radionuclides = new Nuclides();
			lbNuclides.ItemsSource = CalcParams.Source.Radionuclides.Collection;

			//Contex menu
			ContextMenu ctxMenu = new ContextMenu();
			var itemAdd = new MenuItem();
			var itemReplace = new MenuItem();
			var itemRemove = new MenuItem();
			itemAdd.Name = "Add";
			itemReplace.Name = "Change";
			itemRemove.Name = "Remove";
			itemAdd.SetResourceReference(MenuItem.HeaderProperty, "miEdit_Add");
			itemReplace.SetResourceReference(MenuItem.HeaderProperty, "miEdit_Edit");
			itemRemove.SetResourceReference(MenuItem.HeaderProperty, "miEdit_Remove");
			itemAdd.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/add.png", UriKind.Relative)) };
			itemReplace.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/edit.png", UriKind.Relative)) };
			itemRemove.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/remove.png", UriKind.Relative)) };
			ctxMenu.Items.Add(itemAdd);
			ctxMenu.Items.Add(itemReplace);
			ctxMenu.Items.Add(itemRemove);

			itemAdd.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Add, ItemType.Nuclide); };
			itemReplace.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Edit, ItemType.Nuclide); }; 
			itemRemove.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Delete, ItemType.Nuclide); };

			lbNuclides.ContextMenu = ctxMenu;

			sumActivityLabel.IsReadOnly = true;
			IbetaMaxLabel.IsReadOnly = true;

            //set buttons Action handlers
            addNuclideBtn.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Add, ItemType.Nuclide); };
			editNuclideBtn.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Edit, ItemType.Nuclide); };
			removeNuclideBtn.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Delete, ItemType.Nuclide); };
		}

   

        public void RecalcSumActivity()
		{

			sumActivityLabel.Text = CalcParams.Source?.SummaryActivity.ToString();
		}

		/// <summary>
		/// Инициализирует список выбранных материалов защиты
		/// </summary>
		public void InitializeShieldListBox()
		{
			CalcParams.SelectedLayersList = new ShieldLayers();
			lbShielLayers.ItemsSource = CalcParams.SelectedLayersList.Collection;

			//Contex menu
			ContextMenu ctxMenu = new ContextMenu();
			var itemAdd = new MenuItem();
			var itemChange = new MenuItem();
			var itemRemove = new MenuItem();

			itemAdd.SetResourceReference(MenuItem.HeaderProperty, "miEdit_Add");
			itemChange.SetResourceReference(MenuItem.HeaderProperty, "miEdit_Edit");
			itemRemove.SetResourceReference(MenuItem.HeaderProperty, "miEdit_Remove");
			itemAdd.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/add.png", UriKind.Relative)) };
			itemChange.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/edit.png", UriKind.Relative)) };
			itemRemove.Icon = new Image() { Source = new BitmapImage(new Uri("/Resources/Images/remove.png", UriKind.Relative)) };
			ctxMenu		.Items.Add(itemAdd);
			ctxMenu		.Items.Add(itemChange);
			ctxMenu		.Items.Add(itemRemove);

			itemAdd.Click		+= (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Add, ItemType.ShieldLayer); }; 
			itemChange.Click	+= (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Edit, ItemType.ShieldLayer); };
			itemRemove.Click	+= (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Delete, ItemType.ShieldLayer); };

			lbShielLayers.ContextMenu = ctxMenu;

			//Buttons under the list
			addLayerBtn.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Add, ItemType.ShieldLayer); };
			editLayerBtn.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Edit, ItemType.ShieldLayer); };
			removeLayerBtn.Click += (object sender, RoutedEventArgs e) => { HandleListItems(OperationType.Delete, ItemType.ShieldLayer); };
			}


		public void InitializeOutputTab()
		{
			SetIntValueBindingTo(tbDivisionByX, typeof(InputData), "Nx");
			SetIntValueBindingTo(tbDivisionByY, typeof(InputData), "Ny");
			SetIntValueBindingTo(tbDivisionByZ, typeof(InputData), "Nz");

			SetIntValueBindingTo(tbCommaSigns, typeof(CalcParams), "precision", 1, 15);

			SetDoubleValueBindingTo(tbCalcDistance, typeof(CalcParams), "CalculationDistance");

			//Set Tags
			tbDivisionByX.Tag = (string)Application.Current.Resources["tabOutputValue_LabelDivByX"];
			tbDivisionByY.Tag = (string)Application.Current.Resources["tabOutputValue_LabelDivByY"];
			tbDivisionByZ.Tag = (string)Application.Current.Resources["tabOutputValue_LabelDivByZ"];

			tbCommaSigns.Tag = (string)Application.Current.Resources["tabOutputValue_LabelCommaSigns"];
			tbCalcDistance.Tag = (string)Application.Current.Resources["tabOutputValue_LabelCalcDistance"];


			InitializeDoseFactorBoxes();
			InitializeComboboxBuildupCalculationWay();
		}

		public void InitializeDoseFactorBoxes()
		{
			cbOutDoseType.ItemsSource = CalcParams.TableDoseFactors;
			cbOutDoseType.DisplayMemberPath = "ExtendedName";

			Binding bnd = new Binding();
			bnd.ElementName = "cbOutDoseType";
			bnd.Path = new PropertyPath("SelectedValue.FactorData");
			cbDoseFactorGeometry.DisplayMemberPath = "GeometryName";
			cbDoseFactorGeometry.SetBinding(ComboBox.ItemsSourceProperty, bnd);


			Binding bnd2 = new Binding();
			bnd2.ElementName = "cbDoseFactorGeometry";
			bnd2.Path = new PropertyPath("SelectedValue.Value");
			cbEquivalentDoseOrgan.DisplayMemberPath = "Name";
			cbEquivalentDoseOrgan.SetBinding(ComboBox.ItemsSourceProperty, bnd2);
			

			cbOutDoseType.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
			{
				cbDoseFactorGeometry.IsEnabled = false;
				cbEquivalentDoseOrgan.IsEnabled = false;

				if (cbDoseFactorGeometry.Items.Count > 1)
				{
					cbDoseFactorGeometry.IsEnabled = true;
					cbDoseFactorGeometry.SelectedIndex = 0;
				}
			};

			cbDoseFactorGeometry.SelectionChanged += CbDoseFactorGeometry_SelectionChanged;
			

			cbOutDoseType.SelectedIndex = 0;
		}

		private void CbDoseFactorGeometry_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbOutDoseType.SelectedIndex == -1 || cbOutDoseType.SelectedIndex != 1) return;
			
			if (cbEquivalentDoseOrgan.Items.Count > 1)
			{
				cbEquivalentDoseOrgan.IsEnabled = true;
				cbEquivalentDoseOrgan.SelectedIndex = 0;
			}
		}

		public void InitializeComboboxBuildupCalculationWay()
		{
			//FIXME: dynamic update labels
			//SetIntValueBindingTo(tbDivisionByZ, typeof(InputData), "Nz");
			cbCalcBuilupWay.Items.Add((String)Application.Current.Resources["tabOutputValue_BuildupCalcTypeBoxItemJapan"]);
			cbCalcBuilupWay.Items.Add((String)Application.Current.Resources["tabOutputValue_BuildupCalcTypeBoxItemTaylor"]);

			cbCalcBuilupWay.SelectionChanged += CbCalcBuilupWay_SelectionChanged;
			cbCalcBuilupWay.SelectedIndex = 0;
		}

		private void CbCalcBuilupWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbCalcBuilupWay.SelectedIndex == -1) return;
			if (cbCalcBuilupWay.SelectedIndex == 0) Calculation.BuildupCalculationType = Calculation.BuildupCalcType.Toolbox;
			else Calculation.BuildupCalculationType = Calculation.BuildupCalcType.Taylor;
		}

		public void InitializeStatusBar()
		{
			progress = new Progress<int>((i) =>
			{
				progressBar.Value = i;
			});
			progressBar.Maximum = Breamsstrahlung.Length;

			statusbarLabel.SetResourceReference(TextBlock.TextProperty, "msgStatus_Ready");
		}

		public void InitializeOutputBox()
		{
			outputBox.Document.LineHeight = 5;

			ContextMenu menu = new ContextMenu();
			MenuItem iSaveToFile = new MenuItem();
			MenuItem iClear = new MenuItem();
			iSaveToFile.SetResourceReference(MenuItem.HeaderProperty, "ctxMenuSaveToFile");
			iClear.SetResourceReference(MenuItem.HeaderProperty, "ctxMenuClear");

			//Registration of events
			//Event SAVE TO FILE
			iSaveToFile.Click += (object sender, RoutedEventArgs e) =>
			{
				SaveFileDialog dialog = new SaveFileDialog();
				dialog.Filter = "Text File (*.txt)|*.txt|All files (*.*)|*.*";
				dialog.Title = (string)Application.Current.Resources["ctxMenuSaveToFile"];
				var result = dialog.ShowDialog();
				if (result.Value)
				{
					TextRange range = new TextRange(outputBox.Document.ContentStart, outputBox.Document.ContentEnd);
					FileStream file = new FileStream(dialog.FileName, FileMode.Create);
					range.Save(file, System.Windows.DataFormats.Text);
					file.Close();
				}
			};
			//Event CLEAR
			//Clear output box content
			iClear.Click += (object sender, RoutedEventArgs e) => { outputBox.SelectAll(); outputBox.Selection.Text = ""; };

			//Add buttons to context menu
			menu.Items.Add(iSaveToFile);
			menu.Items.Add(iClear);
			//Add context menu to output box
			outputBox.ContextMenu = menu;
		}

		private void SetIntValueBindingTo(TextBox sender, Type source, string path, int min = 1, int max = 10000)
		{
			Binding bnd = new Binding();
			
			//bnd.Source = typeof(InputData);
			bnd.Path = new PropertyPath("(0)", source.GetProperty(path));
			bnd.Mode = BindingMode.TwoWay;

			bnd.NotifyOnValidationError = true;
			//bnd.ValidatesOnDataErrors = true;
			//bnd.ValidatesOnNotifyDataErrors = true;
			bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			var rule = new ValidIntMinMaxRule();
			rule.Min = min; rule.Max = max;

			bnd.ValidationRules.Add(rule);
			//bnd.ValidationRules.Add(new ExceptionValidationRule());
			rule.ValidatesOnTargetUpdated = true;
			rule.ValidationStep = ValidationStep.RawProposedValue;
			sender.SetBinding(TextBox.TextProperty, bnd);
		}

		private void SetDoubleValueBindingTo(TextBox sender, Type source, string path)
		{
			Binding bnd = new Binding();

			bnd.Path = new PropertyPath("(0)", source.GetProperty(path));
			bnd.Mode = BindingMode.TwoWay;
			bnd.Delay = 500;
			bnd.StringFormat = "{0:G}";

			bnd.NotifyOnValidationError = true;
			bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			var rule = new ValidDoubleWithMinRule();
			rule.Min = 0.0;

			bnd.ValidationRules.Add(rule);
			//bnd.ValidationRules.Add(new ExceptionValidationRule());
			rule.ValidatesOnTargetUpdated = true;
			rule.ValidationStep = ValidationStep.RawProposedValue;
			sender.SetBinding(TextBox.TextProperty, bnd);
		}
		private void SetDoubleValueBindingTo(TextBox sender, object source, string path, BindingMode mode  = BindingMode.TwoWay)
		{
			Binding bnd = new Binding();

			bnd.Source = source;
			bnd.Path = new PropertyPath(path);
			bnd.Mode = mode;
			bnd.Delay = 500;
			bnd.StringFormat = "{0:G}";
			//bnd.Converter = new ConverterDouble();

			bnd.NotifyOnValidationError = true;
			bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			var rule = new ValidDoubleWithMinRule();
			rule.Min = 0.0;

			bnd.ValidationRules.Add(rule);
			//bnd.ValidationRules.Add(new ExceptionValidationRule());
			rule.ValidatesOnTargetUpdated = true;
			rule.ValidationStep = ValidationStep.RawProposedValue;
			sender.SetBinding(TextBox.TextProperty, bnd);
		}

		private void SetDoubleValueBindingTo(TextBox sender, string elementName, string path, BindingMode mode)
		{
			Binding bnd = new Binding();

			bnd.ElementName = elementName;
			bnd.Path = new PropertyPath(path);
			bnd.Mode = mode;
			bnd.Delay = 500;
			bnd.StringFormat = "{0}";
			//bnd.Converter = new ConverterDouble();

			bnd.NotifyOnValidationError = true;
			bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			var rule = new ValidDoubleWithMinRule();
			rule.Min = 0.0;

			bnd.ValidationRules.Add(rule);
			//bnd.ValidationRules.Add(new ExceptionValidationRule());
			rule.ValidatesOnTargetUpdated = true;
			rule.ValidationStep = ValidationStep.RawProposedValue;
			sender.SetBinding(TextBox.TextProperty, bnd);
		}

		public void HandleListItems(OperationType operation, ItemType type)
        {
			if (operation == OperationType.Add)
            {
				if (type == ItemType.Nuclide)
                {
					popupComboBox.SelectedIndex = 0;
					popupTextBox.Text = "0.0E+000";
					popupAddNuclide.Tag = OperationType.Add;
					popupAddNuclide.IsOpen = true;
				}
				else if (type == ItemType.ShieldLayer)
                {
					cbPopupAddLayer.SelectedIndex = 0;
					tbPopupD.Text = "1.0";
					popupAddLayer.Tag = OperationType.Add;
					popupAddLayer.IsOpen = true;
				}
            }
			if (operation == OperationType.Edit)
			{
				
				if (type == ItemType.Nuclide && lbNuclides.SelectedIndex != -1)
				{
					var nuclide = lbNuclides.SelectedItem as Nuclide;
					popupComboBox.SelectedIndex = CalcParams.TableNuclides.IndexOf(nuclide.Name);
					popupTextBox.Text = string.Format("{0:G3}", nuclide.Activity.ToString());
					popupAddNuclide.Tag = OperationType.Edit;
					popupAddNuclide.IsOpen = true;
				}
              
				if (type == ItemType.ShieldLayer && lbShielLayers.SelectedIndex != -1)
				{
					var layer = lbShielLayers.SelectedItem as MaterialLayer;
					if (layer == null)
					{
						layer = new MaterialLayer(CalcParams.TableMaterials[0]);
						layer.d = 1.0;
						layer.Density = CalcParams.TableMaterials[0].Density;
					}
					else
					{
						cbPopupAddLayer.SelectedIndex = CalcParams.TableMaterials.IndexOf(layer.Material.Name);
						tbPopupD.Text = layer.d.ToString();
						tbPopupDensity.Text = layer.Density.ToString();
					}

					popupAddLayer.Tag = OperationType.Edit;
					popupAddLayer.IsOpen = true;
				}
			}
			if (operation == OperationType.Delete)
			{
				try
				{
					if (type == ItemType.Nuclide)
					{
						var nuclide = lbNuclides.SelectedItem as Nuclide;
						CalcParams.Source.Radionuclides.RemoveNuclide(nuclide);
					}
					else if (type == ItemType.ShieldLayer)
					{
						var layer = lbShielLayers.SelectedItem as MaterialLayer;
						CalcParams.SelectedLayersList.RemoveLayer(layer);
					}
				}
				catch
				{
					MessageBox.Show((string)Application.Current.Resources["msgError_NothingDelete"], (string)Application.Current.Resources["msgWarning_Title"], MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				
			}
		}
	}
}
