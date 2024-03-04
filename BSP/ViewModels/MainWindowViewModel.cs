using BSP.BL.Calculation;
using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using BSP.Source.XAML_Forms;
using BSP.ViewModels.Tabs;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
            ShieldingTab = new(materialsService);
            DoseFactorsTab = new(dcfService);
            LanguagesVM = new();

            string progressLabel = (string)Application.Current.Resources["msg_Progress"] ?? "Progress";
            progress = new Progress<int>(i =>
            {
                ProgressValue = i;
                ShowStatusMessage($"{progressLabel} {i}%");
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
        private bool isPointSource = false;
        private string resultsText;

        private float _calculationDistance = 100;
       
        private MaterialDto _selectedEnvironmentMaterial;
        private CancellationTokenSource tokenSource;

        private int progressValue = 0;
        private IProgress<int> progress { get; }
        #endregion

        #region Properties
        public int Precision { get => precision; set { precision = value > 0 && value < 10 ? value : 3; OnChanged(); } }
        public bool IsShowPartialDoseRates { get => isShowPartialDoseRates; set { isShowPartialDoseRates = value; OnChanged(); } }
        public bool IsPointSource { get => isPointSource; set { isPointSource = value; OnChanged(); } }
        public string ResultsText { get => resultsText; set { resultsText = value; OnChanged(); } }

        public float CalculationDistance { get => _calculationDistance; set { _calculationDistance = value; OnChanged(); } }

        public MaterialDto? SelectedEnvironmentMaterial { get => _selectedEnvironmentMaterial; set { _selectedEnvironmentMaterial = value; OnChanged(); } }
        /// <summary>
        /// Контролирует доступность элементов управления во время вычислений
        /// </summary>
        public bool IsEvaluationInProgress { get => isEvaluationInProgress; set { isEvaluationInProgress = value; OnChanged(); OnChanged(nameof(IsEvaluationNotInProgress)); } }

        public bool IsEvaluationNotInProgress { get => !isEvaluationInProgress; }

        /// <summary>
        /// Текущий прогресс вычислений
        /// </summary>
        public int ProgressValue { get => progressValue; set { progressValue = value; OnChanged(); } }
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

        public RelayCommand StartCommand => startCommand ?? (startCommand = new RelayCommand(obj => StartCalculation(), o => !IsEvaluationInProgress));
        public RelayCommand StopCommand => stopCommand ?? (stopCommand = new RelayCommand(obj => StopCalculation(), o => IsEvaluationInProgress));
        public RelayCommand ClearResultsCommand => clearResultsCommand ?? (clearResultsCommand = new RelayCommand(o => ClearResultsView()));
        public RelayCommand ExportResultsToTextFile => exportResultsToTextFile ?? (exportResultsToTextFile = new RelayCommand(o => ExportResults(), o => !string.IsNullOrEmpty(resultsText)));

        public RelayCommand AboutCommand => new RelayCommand(o => new About().ShowDialog());
        public RelayCommand UserManualCommand => new RelayCommand(o =>
        {
            if (File.Exists("UserManual.pdf"))
                Process.Start(new ProcessStartInfo() { UseShellExecute = true, FileName = "UserManual.pdf" });
        });
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

            if (CalculationDistance <= 0)
                base.AddError((res.TryFindResource("msg_Error_ZeroCalculationDistance") as string) ?? "Calculation distance have to be greater zero");

            if (Precision <= 0)
                base.AddError((res.TryFindResource("msg_Error_ZeroOrLessDecimalPlaces") as string) ?? "Number of decimal places is less or equal zero");

            return IsValid;
        }
        #endregion


        #region StartCalculation
        public async Task StartCalculation()
        {
            base.ShowStatusMessage((string)Application.Current.Resources["msg_Status_ValidationProcess"] ?? "Validation in progress");

            if (ValidateInputs())
            {
                //Блокируем интерфейс
                IsEvaluationInProgress = true;

                var shieldLayersIds = ShieldingTab.ShieldLayers.Select(s => s.Id).ToArray();
                tokenSource = new CancellationTokenSource();

                //Bremsstrahlung
                //Выбираем только пары значений энергии и интенсивности, в которых значения отличны от нуля и выше порога 0,015 МэВ. Затем сортируем по возрастанию энергии.
                var energyIntensityData = SourceTab.EnergyYieldList
                    .Where(ei => ei.Energy > 0 && ei.EnergyYield > 0 && ei.Energy > SourceTab.CutoffBremsstrahlungEnergy)
                    .OrderBy(ei => ei.Energy)
                    .ToList();

                var energies = energyIntensityData.Select(e => e.Energy).ToArray();
                var bremsstrahlungEnergyYields = energyIntensityData.Select(y => y.EnergyYield).ToArray();
                //Сортируем массивы по возрастанию энергии, сохраняя связь значений
                Array.Sort(energies, bremsstrahlungEnergyYields);

                InputData input = App.GetService<InputDataBuilder>()
                    .WithShieldLayers(ShieldingTab.ShieldLayers.ToList())
                    .WithAttenuationFactors(SourceTab.SelectedSourceMaterial.Id, shieldLayersIds, energies)
                    .WithEnvironmentAbsorptionFactors(energies, SelectedEnvironmentMaterial?.Id ?? 1)
                    .WithBremsstrahlungYields(bremsstrahlungEnergyYields)
                    .WithBuildup(
                        BuildupTab.SelectedBuildup.BuildupType,
                        BuildupTab.IsIncludeBuildup ? BuildupTab.SelectedComplexBuildup.BuildupType : null,
                        SourceTab.SelectedSourceMaterial.Id,
                        shieldLayersIds, energies)
                    .WithDoseConversionFactors(DoseFactorsTab.SelectedDoseFactorType.DoseFactorType, energies, DoseFactorsTab.SelectedExposureGeometry.Id, DoseFactorsTab.SelectedOrganTissue.Id)
                    .WithSourceActivityAndDensity(SourceTab.SourceTotalActivity, SourceTab.SourceDensity)
                    .WithCalculationPoint(CalculationDistance)
                    .WithCancellationToken(this.tokenSource.Token)
                    .WithProgress(progress)
                    .Build();

                //Source form processor
                var dimensions = SourceTab.SourceDimensions.Select(d => d.Value).ToArray();
                var discreteness = SourceTab.SourceDimensions.Select(d => d.Discreteness).ToArray();
                var formProcessor = GeometryService.GetGeometryInstance(SourceTab.SelectedSourceForm.FormType, dimensions, discreteness);

                var results = await Calculation.StartAsync(input, formProcessor);
                FillOutputTable(results);

                IsEvaluationInProgress = false;
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
        }
        #endregion

        #region StopCalculation
        public void StopCalculation()
        {
            this.tokenSource.Cancel();
            ShowStatusMessage((string)Application.Current.Resources["msg_Status_Cancel"] ?? "Cancelled by user");
        }
        #endregion


        #region FillOutputTable
        private void FillOutputTable(OutputValue results)
        {
            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            //CalculationDate   Form    Material    Distance    DoseRate    Units
            string generalFormat = "{0,-20:dd.MM.yyyy HH:mm}{1,-30}{2,30}{3,10} cm{4,15:e3} {5}\n";

            //Energy    DoseRate    Units
            string partialDataFormat = "\t{0,10:0.####}\t{1,10:e3}\t{2}\n";


            //Запрашиваем единицы измерения дозы
            var units = DoseFactorsService.GetUnits(DoseFactorsTab.SelectedDoseFactorType.DoseFactorType);
            units += "/h";

            //Заполняем заголовок результатов
            ResultsText += string.Format(generalFormat,
                Application.Current.Resources["ResultsView_LaunchTim"] ?? "Launch time",
                Application.Current.Resources["ResultsView_SourceForm"] ?? "Form",
                Application.Current.Resources["ResultsView_SourceMaterial"] ?? "Material",
                Application.Current.Resources["ResultsView_CalculationPoint"] ?? "Distance from source",
                Application.Current.Resources["ResultsView_DoseRate"] ?? "Dose rate",
                "");

            ResultsText += string.Format(generalFormat,
                DateTime.Now,
                SourceTab.SelectedSourceForm.Name,
                SourceTab.SelectedSourceMaterial.Name,
                CalculationDistance,
                results.TotalDoseRate,
                units);

            if (isShowPartialDoseRates)
            {
                ResultsText += string.Format(partialDataFormat,
                    Application.Current.Resources["ResultsView_Energy"] ?? "Energy, MeV",
                    Application.Current.Resources["ResultsView_DoseRate"] ?? "Dose rate",
                    "");

                for (var i = 0; i < results.PartialDoseRates.Length; i++)
                {
                    ResultsText += string.Format(partialDataFormat,
                    SourceTab.EnergyYieldList[i].Energy,
                    results.PartialDoseRates[i],
                    units);
                }
            }
        }
        #endregion

        private void ClearResultsView()
        {
            ResultsText = "";
        }

        private void ExportResults()
        {

        }

        private void ResetProgress()
        {
            progress.Report(0);
        }

        #region IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(CalculationDistance):
                        if (CalculationDistance <= 0)
                            error = (Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Incorrect distance value";
                        break;
                }

                return error;
            }
        }
        public string Error => string.Empty; 
        #endregion
    }
}
