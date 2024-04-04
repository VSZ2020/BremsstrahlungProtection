using BSP.BL.Calculation;
using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using BSP.Geometries.SDK;
using BSP.Source.XAML_Forms;
using BSP.ViewModels.Tabs;
using BSP.Views;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows;

namespace BSP.ViewModels
{
    public class MainWindowViewModel : BaseNotificationViewModel, IDataErrorInfo
    {
        #region Constructor
        public MainWindowViewModel(RadionuclidesService radionuclidesService, MaterialsService materialsService, DoseFactorsService dcfService)
        {
            DataController = new AvailableDataController(radionuclidesService, materialsService);

            SourceTab = new(radionuclidesService, DataController);
            BuildupTab = new();
            ShieldingTab = new();
            DoseFactorsTab = new(dcfService);
            LanguagesVM = new();

            string progressLabel = (string)Application.Current.Resources["msg_Progress"] ?? "Progress";
            progress = new Progress<double>(i =>
            {
                ProgressValue = i;
                //ShowStatusMessage($"{progressLabel} {i}%");
            });

            SelectedEnvironmentMaterial = AvailableDataController.AvailableMaterials.FirstOrDefault();

            ResultsText = ((Application.Current.TryFindResource("msg_Welcome") as string) ?? "Welcome to the 'Bremsstrahlung protection' computer code!") + "\n";

        }
        #endregion

        #region Fields
        private bool hasSelectedRadionuclides => SourceTab.SelectedRadionuclides.Count > 0;

        private bool isEvaluationInProgress = false;
        private int precision = 3;
        private bool isShowPartialDoseRates = false;
        private bool isSelftabsorptionOff = false;

        private string resultsText;

        private MaterialDto _selectedEnvironmentMaterial;
        private CancellationTokenSource tokenSource;

        private double progressValue = 0;
        private IProgress<double> progress { get; }
        #endregion

        #region Properties
        public int Precision { get => precision; set { precision = value > 0 && value < 10 ? value : 3; OnChanged(); } }
        public bool IsShowPartialDoseRates { get => isShowPartialDoseRates; set { isShowPartialDoseRates = value; OnChanged(); } }
        public bool IsSelfAbsorptionOff { get => isSelftabsorptionOff; set { isSelftabsorptionOff = value; OnChanged(); } }

        public string ResultsText { get => resultsText; set { resultsText = value; OnChanged(); } }

        public MaterialDto? SelectedEnvironmentMaterial { get => _selectedEnvironmentMaterial; set { _selectedEnvironmentMaterial = value; OnChanged(); } }
        /// <summary>
        /// Контролирует доступность элементов управления во время вычислений
        /// </summary>
        public bool IsEvaluationInProgress { get => isEvaluationInProgress; set { isEvaluationInProgress = value; OnChanged(); OnChanged(nameof(IsEvaluationNotInProgress)); } }

        public bool IsEvaluationNotInProgress { get => !isEvaluationInProgress; }

        /// <summary>
        /// Текущий прогресс вычислений
        /// </summary>
        public double ProgressValue { get => progressValue; set { progressValue = value; OnChanged(); } }
        #endregion

        #region ViewModels
        public AvailableDataController DataController { get; }
        public SourceTabVM SourceTab { get; }
        public BuildupTabVM BuildupTab { get; }
        public ShieldingTabVM ShieldingTab { get; }
        public DoseFactorsTabVM DoseFactorsTab { get; }

        public LanguageVM LanguagesVM { get; }
        #endregion

        #region Commands
        RelayCommand startCommand;
        RelayCommand stopCommand;
        RelayCommand clearResultsCommand;
        RelayCommand exportResultsToTextFile;
        RelayCommand showInterpolatedDataViewer;
        RelayCommand showRadionuclidesViewerCommand;

        public RelayCommand StartCommand => startCommand ?? (startCommand = new RelayCommand(obj => ButtonStartClicked(), o => !IsEvaluationInProgress));
        public RelayCommand StopCommand => stopCommand ?? (stopCommand = new RelayCommand(obj => StopCalculation(), o => IsEvaluationInProgress));
        public RelayCommand ClearResultsCommand => clearResultsCommand ?? (clearResultsCommand = new RelayCommand(o => ClearResultsView()));
        public RelayCommand ExportResultsToTextFile => exportResultsToTextFile ?? (exportResultsToTextFile = new RelayCommand(o => ExportResults(), o => !string.IsNullOrEmpty(resultsText)));
        public RelayCommand ShowInterpolatedDataViewerCommand => showInterpolatedDataViewer ?? (showInterpolatedDataViewer = new RelayCommand(o => ShowInterpolationsViewer()));
        public RelayCommand ShowRadionuclidesViewerCommand => showRadionuclidesViewerCommand ?? (showRadionuclidesViewerCommand = new RelayCommand(o => ShowRadionuclidesViewer()));

        public RelayCommand AboutCommand => new RelayCommand(o => new About().ShowDialog());
        public RelayCommand UserManualCommand => new RelayCommand(o =>
        {
            if (File.Exists("UserManual.pdf"))
                Process.Start(new ProcessStartInfo() { UseShellExecute = true, FileName = "UserManual.pdf" });
        });

        public RelayCommand ShowChangelogCommand => new RelayCommand(o =>
        {
            if (File.Exists("Changelog.txt"))
                Process.Start(new ProcessStartInfo() { FileName = "Changelog.txt", UseShellExecute = true });
            else
                MessageBox.Show("Changelog file is not found");
        });

        public RelayCommand ExitCommand => new RelayCommand(o => Environment.Exit(0));
        #endregion


        #region ValidateInputs
        /// <summary>
        /// Валидация входных данных формы
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {

            base.ClearValidationMessages();
            var res = Application.Current;


            if (!hasSelectedRadionuclides && SourceTab.IsAutoGeneratedModeChecked)
                base.AddError((res.TryFindResource("msg_Error_NoSelectedRadionuclides") as string) ?? "No one radionuclide was selected for calculations");

            if (SourceTab.SourceDimensions.Select(d => d.Discreteness).Count(d => d == 0) > 0)
                base.AddError((res.TryFindResource("msg_Error_DimensionDiscreteness") as string) ?? "Dimension discreteness should be greated zero");

            if (SourceTab.SourceDimensions.Select(d => d.Value).Count(d => d == 0) > 0)
                base.AddError((res.TryFindResource("msg_Error_ZeroDimension") as string) ?? "Dimension should be greated zero");

            if (SourceTab.EnergyYieldList.Count == 0)
                base.AddError((string)res.Resources["msg_Error_EmptyEnergyYieldData"] ?? "Energy and Yield data are empty. Click update to generate data or enter it manually");

            if (SourceTab.SourceTotalActivity < 1)
                base.AddError((res.TryFindResource("msg_Error_ZeroTotalActivity") as string) ?? "Source total activity is zero or empty");

            if (Precision <= 0)
                base.AddError((res.TryFindResource("msg_Error_ZeroOrLessDecimalPlaces") as string) ?? "Number of decimal places is less or equal zero");

            var sourceWidth = GeometryService.GetSubstractionTermForAirgapCalculation(SourceTab.SelectedSourceForm.FormType, SourceTab.SourceDimensions.Select(d => d.Value).ToArray());
            var shieldsWidth = ShieldingTab.ShieldLayers.Select(l => l.D).Sum();
            var totalWidth = sourceWidth + shieldsWidth;
            if (SourceTab.DosePoints.Any(dp => dp.X <= totalWidth))
                base.AddError((res.TryFindResource("msg_Error_DosePointInsideSourceOrShield") as string) ?? "Dose point X coordinate is inside source and shielding thickness");

            return SourceTab.ValidateModel(this) && IsValid;
        }
        #endregion

        #region GetBuilder
        private InputDataBuilder GetBuilder(double[] energies, double[] bremsstrahlungEnergyFluxes)
        {
            //Обновляем экземпляр токена отмены операции
            tokenSource = new CancellationTokenSource();

            var shields = ShieldingTab.ShieldLayers.Select(l => new ShieldLayer()
            {
                Id = l.Id,
                Z = l.Z,
                D = l.D,
                Name = l.Name,
                Weight = l.Weight,
                Density = l.Density,
            }).ToList();

            //Добавляем последним слоем слой среды
            shields.Add(new ShieldLayer()
            {
                Id = SelectedEnvironmentMaterial!.Id,
                Density = SelectedEnvironmentMaterial.Density,
            });

            //Выбираем идентификаторы слоев защиты
            var shieldLayersIds = shields.Select(s => s.Id).ToArray();

            var builder = App.GetService<InputDataBuilder>()
                    .WithDimensions(
                        SourceTab.SourceDimensions.Select(d => d.Value).ToArray(),
                        SourceTab.SourceDimensions.Select(d => d.Discreteness).ToArray())
                    .WithEnergies(energies)
                    .WithShieldLayers(shields)
                    .WithAttenuationFactors(SourceTab.SelectedSourceMaterial.Id, shieldLayersIds, energies)
                    .WithEnvironmentAbsorptionFactors(energies, SelectedEnvironmentMaterial?.Id ?? 1)
                    .WithBremsstrahlungEnergyFluxes(bremsstrahlungEnergyFluxes)
                    .WithBuildup(
                        BuildupTab.SelectedBuildup.BuildupType,
                        BuildupTab.IsIncludeBuildup ? BuildupTab.SelectedComplexBuildup.BuildupType : null,
                        SourceTab.SelectedSourceMaterial.Id,
                        shieldLayersIds, energies)
                    .WithSourceDensity(SourceTab.SourceDensity)
                    .WithSourceActivity(SourceTab.SourceTotalActivity)
                    .WithCancellationToken(tokenSource.Token)
                    .WithProgress(progress)
                    .WithSelfabsorption(!IsSelfAbsorptionOff);
            return builder;
        }
        #endregion

        #region GetDoseFactors
        private double[] GetDoseFactors(double[] energies)
        {
            return App.GetService<DoseFactorsService>().GetDoseConversionFactors(
                    DoseFactorsTab.SelectedDoseFactorType.DoseFactorType,
                    energies,
                    DoseFactorsTab.SelectedExposureGeometry.Id,
                    DoseFactorsTab.SelectedOrganTissue.Id);
        }
        #endregion

        #region ButtonStartClicked
        public async void ButtonStartClicked()
        {
            base.ShowStatusMessage((string)Application.Current.Resources["msg_Status_ValidationProcess"] ?? "Validation in progress");

            if (ValidateInputs())
            {
                //Блокируем интерфейс
                IsEvaluationInProgress = true;
                ResetProgress();

                await EvaluateByPointAsync();

                base.ShowStatusMessage("Completed!");
            }
            else
            {
                ShowStatusMessage((Application.Current.Resources["msg_Status_ValidationProcessError"] as string) ?? "Has inputs errors!");
                var msgs = base.ValidationMessagesToString();
                MessageBox.Show(
                    ((Application.Current.Resources["msg_Error_CalculationCantBeStarted"] as string) ?? "Calculation can't be launched due to error(s):") +
                    "\n" + msgs);
            }

            ResetProgress();
            IsEvaluationInProgress = false;
        }
        #endregion

        #region StopCalculation
        public void StopCalculation()
        {
            this.tokenSource.Cancel();
            ShowStatusMessage((string)Application.Current.Resources["msg_Status_Cancel"] ?? "Cancelled by user");
        }
        #endregion

        #region EvaluateByPointAsync
        private async Task EvaluateByPointAsync(InputDataBuilder? builder = null, bool isMutedOutput = false)
        {
            //Данные по спектру тормозного излучения
            (double[] energies, double[] bremsstrahlungEnergyFluxes) = SourceTab.GetBremsstrahlungSpectrum();

            if (builder == null)
                builder = GetBuilder(energies, bremsstrahlungEnergyFluxes);

            var doseFactors = GetDoseFactors(energies);

            var dimensions = SourceTab.SourceDimensions.Select(d => d.Value).ToArray();
            var formProcessor = GeometryService.GetGeometryInstance(SourceTab.SelectedSourceForm.FormType);
            var airgapSubstractionTerm = GeometryService.GetSubstractionTermForAirgapCalculation(SourceTab.SelectedSourceForm.FormType, dimensions);
            var shieldsTotalLengthWithoutAirgap = ShieldingTab.ShieldLayers.Select(l => l.D).Sum();

            var dosePointsResults = new List<OutputValue>();
            //Выполняем расчет для каждой точки регистрации излучения
            var targetPoints = SourceTab.DosePoints.Select(p => new Vector3(p.X, p.Y, p.Z)).ToArray();
            foreach (var point in targetPoints)
            {
                base.ShowStatusMessage(string.Format("Evaluation for dose point ({0},{1},{2})", point.X, point.Y, point.Z));
                var input = builder
                    .WithCalculationPoint(point)
                    .Build();

                //Рассчитываем толщину слоя воздуха
                input.Layers.Last().D = point.X - shieldsTotalLengthWithoutAirgap - airgapSubstractionTerm;

                progress?.Report(0);
                var results = await Calculation.StartAsync(input, formProcessor);

                if (input.CancellationToken.IsCancellationRequested)
                {
                    isMutedOutput = true;
                    break;
                }

                results.PartialAirKerma = results.ConvertToAnotherDose(doseFactors);
                dosePointsResults.Add(results);
            }

            if (!isMutedOutput)
                FillOutputTable(dosePointsResults, this.precision);
        }
        #endregion

        #region FillOutputTable
        private void FillOutputTable(List<OutputValue> results, int precise = 3)
        {
            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            string dosePointFormat = "({0},{1},{2})";
            //CalculationDate   Form    Material    Distance    DoseRate    Units
            string generalFormat = "{0,-20:dd.MM.yyyy HH:mm}{1,-30}{2,30}{3,20}{4,15:e" + precise + "} {5}\n";

            //Energy    FluxDensity     EnergyFluxDensity   DoseRate    Units
            string partialDataFormat = "{0:e" + precise + "}\t{1:e" + precise + "}\t{2:e" + precise + "}\t{3:e" + precise + "}\t{4}\n";


            //Запрашиваем единицы измерения дозы
            var units = DoseFactorsService.GetUnits(DoseFactorsTab.SelectedDoseFactorType.DoseFactorType);
            units += "/h";

            //Заполняем заголовок результатов
            ResultsText += string.Format(generalFormat,
                Application.Current.Resources["ResultsView_LaunchTim"] ?? "Launch time",
                Application.Current.Resources["ResultsView_SourceForm"] ?? "Form",
                Application.Current.Resources["ResultsView_SourceMaterial"] ?? "Source Material",
                Application.Current.Resources["ResultsView_DosePoint"] ?? "Dose point",
                Application.Current.Resources["ResultsView_DoseRate"] ?? "Dose rate",
                "");

            foreach (var result in results)
            {
                ResultsText += string.Format(generalFormat,
                DateTime.Now,
                SourceTab.SelectedSourceForm.Name,
                SourceTab.SelectedSourceMaterial.Name,
                string.Format(dosePointFormat, result.DosePoint.X, result.DosePoint.Y, result.DosePoint.Z),
                result.TotalDoseRate,
                units);

                if (isShowPartialDoseRates)
                {
                    ResultsText += string.Format(partialDataFormat,
                        Application.Current.Resources["ResultsView_Energy"] ?? "Energy, MeV",
                        Application.Current.Resources["ResultsView_PhotonsFluxDensity"] ?? "Flux Density, 1/cm2/s",
                        Application.Current.Resources["ResultsView_EnergyFluxDensity"] ?? "Energy Flux Density, MeV/cm2/s",
                        Application.Current.Resources["ResultsView_DoseRate"] ?? "Dose rate",
                        "");

                    for (var i = 0; i < result.PartialAirKerma.Length; i++)
                    {
                        ResultsText += string.Format(partialDataFormat,
                        result.Energies[i],
                        result.PartialFluxDensity[i],
                        result.PartialFluxDensity[i] * result.Energies[i],
                        result.PartialAirKerma[i],
                        units);
                    }
                }
            }
            ResultsText += "\n";
        }
        #endregion

        #region ClearResultsView
        private void ClearResultsView()
        {
            ResultsText = "";
        }
        #endregion

        #region ExportResults
        private void ExportResults()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.AddExtension = true;
            dialog.DefaultExt = ".txt";
            var results = dialog.ShowDialog();
            if (results.HasValue && results.Value)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, ResultsText);
                    MessageBox.Show($"File was successfully saved at {dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"File is not saved. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        #endregion

        #region ShowInterpolationsViewer
        private void ShowInterpolationsViewer()
        {
            if (SourceTab.EnergyYieldList.Count == 0)
            {
                MessageBox.Show(Application.Current.TryFindResource("msg_ValidationNoGroupedEnergies") as string, Application.Current.TryFindResource("msgWarning_Title") as string, button: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                return;
            }

            if (SourceTab.SelectedSourceMaterial == null)
            {
                MessageBox.Show(Application.Current.TryFindResource("msg_ValidationNoSourceMaterial") as string, Application.Current.TryFindResource("msgWarning_Title") as string, button: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                return;
            }

            if (SelectedEnvironmentMaterial == null)
            {
                MessageBox.Show(Application.Current.TryFindResource("msg_ValidationNoEnvironmentMaterial") as string, Application.Current.TryFindResource("msgWarning_Title") as string, button: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                return;
            }

            if (DoseFactorsTab.SelectedDoseFactorType == null)
            {
                MessageBox.Show(Application.Current.TryFindResource("msg_ValidationNoDoseFactorType") as string, Application.Current.TryFindResource("msgWarning_Title") as string, button: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                return;
            }

            if (BuildupTab.SelectedBuildup == null || BuildupTab.SelectedComplexBuildup == null)
            {
                MessageBox.Show(Application.Current.TryFindResource("msg_ValidationNoBuildupFactorType") as string, Application.Current.TryFindResource("msgWarning_Title") as string, button: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                return;
            }

            var selectedMaterialsIds = new List<int>() { SourceTab.SelectedSourceMaterial.Id };
            foreach (var shield in ShieldingTab.ShieldLayers)
            {
                if (selectedMaterialsIds.Contains(shield.Id))
                    continue;

                selectedMaterialsIds.Add(shield.Id);
            }

            var wnd = new InterpolationsViewer(
                SourceTab.EnergyYieldList.Select(e => (double)e.Energy).ToArray(),
                SelectedEnvironmentMaterial.Id,
                selectedMaterialsIds.ToArray(),
                BuildupTab.SelectedBuildup.BuildupType,
                App.GetService<MaterialsService>(),
                App.GetService<BuildupService>(),
                App.GetService<DoseFactorsService>(),
                DoseFactorsTab.SelectedDoseFactorType.DoseFactorType,
                DoseFactorsTab.SelectedExposureGeometry.Id,
                DoseFactorsTab.SelectedOrganTissue.Id
                );
            wnd.Owner = Application.Current.MainWindow;
            wnd.ShowDialog();
        }
        #endregion

        #region ShowRadionuclidesViewer
        private void ShowRadionuclidesViewer()
        {
            new Views.RadionuclidesViewer(App.GetService<RadionuclidesService>()).ShowDialog();
        }
        #endregion

        #region ResetProgress
        private void ResetProgress()
        {
            progress.Report(0);
        }
        #endregion

        #region IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Precision):
                        if (Precision <= 0)
                            error = (Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Incorrect precision value";
                        break;
                }

                return error;
            }
        }
        public string Error => string.Empty;
        #endregion
    }
}
