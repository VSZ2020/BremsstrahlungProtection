using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using BSP.Views;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Windows;

namespace BSP.ViewModels.InterpolatedDataViewer
{
    /// <summary>
    /// Пример плохого кода, но это работает
    /// </summary>
    public class InterpolationViewerVM : BaseViewModel
    {
        public InterpolationViewerVM(double[] bremsstrahlungEnergies, int selectedEnvironmentMaterialId, int[] selectedMaterialsIds, Type selectedBuildupType, MaterialsService materialsService, BuildupService buildupService, DoseFactorsService doseFactorsService, Type selectedDoseFactorType, int exposureGeometryId, int OrganTissueId)
        {
            this._energies = bremsstrahlungEnergies;
            this._selectedBuildupType = selectedBuildupType;

            this._materialsService = materialsService;
            this._buildupService = buildupService;

            _environmentMaterial = materialsService.GetMaterialById(selectedEnvironmentMaterialId);
            _userMaterials = materialsService.GetMaterialsById(selectedMaterialsIds);
            AvailableMaterials = new ObservableCollection<MaterialDto>(_userMaterials);

            AvailableBuildupCoefficients = new(BuildupService.GetBuildupCoefficientsNames(selectedBuildupType));

            AvailableParamaters = new List<InterpolatedParameterType>()
            {
                InterpolatedParameterType.AttenuationFactors,
                InterpolatedParameterType.AbsorptionFactors,
                InterpolatedParameterType.BuildupFactors,
                InterpolatedParameterType.DoseConversionFactors
            };

            //Заранее интерполируем и заполняем данные по коэффициентам конверсии
            (_tableDoseFactorsEnergies, _tableDoseFactorsValues) = doseFactorsService.GetTableDoseConversionFactors(selectedDoseFactorType, exposureGeometryId, OrganTissueId);
            _interpolatedDoseFactors = doseFactorsService.GetDoseConversionFactors(selectedDoseFactorType, bremsstrahlungEnergies, exposureGeometryId, OrganTissueId);
            var doseFactorTranslation = Application.Current.TryFindResource(DoseFactorsService.GetTranslationKey(selectedDoseFactorType));
            _doseFactorName = doseFactorTranslation != null ? (string)doseFactorTranslation : "Dose conversion factor";
            _doseFactorUnits = DoseFactorsService.GetDoseConversionFactorUnits(selectedDoseFactorType);

            PlotModel = new PlotModel() { IsLegendVisible = true };

            SelectedParameterType = AvailableParamaters.FirstOrDefault();
            SelectedMaterial = AvailableMaterials.FirstOrDefault();
            SelectedBuildupCoefficient = AvailableBuildupCoefficients.FirstOrDefault();
        }

        #region Commands
        private RelayCommand exportToExcel;
        public RelayCommand ExportToExcelCommand => exportToExcel ?? (exportToExcel = new RelayCommand(o => ExportToExcel(), o => false));

        private RelayCommand openTableView;
        public RelayCommand OpenTableViewCommand => openTableView ?? (openTableView = new RelayCommand(o => OpenTableView(), CanExecute));

        private RelayCommand resetScaleCommand;
        public RelayCommand ResetScaleCommand => resetScaleCommand ?? (resetScaleCommand = new RelayCommand(o => ResetScale()));
        #endregion

        #region Properties and fields
        private readonly MaterialsService _materialsService;
        private readonly BuildupService _buildupService;

        private string _doseFactorName;
        private readonly string _doseFactorUnits;
        private readonly double[] _tableDoseFactorsEnergies;
        private readonly double[] _tableDoseFactorsValues;
        private readonly double[] _interpolatedDoseFactors;

        private readonly Type _selectedBuildupType;
        private readonly List<MaterialDto> _userMaterials;
        private readonly MaterialDto _environmentMaterial;

        private bool _isMaterialsListEnabled = true;
        private bool _isBuildupFactorsListEnabled = true;


        private InterpolatedParameterType? _selectedParameterType;
        public InterpolatedParameterType? SelectedParameterType { get => _selectedParameterType; set { _selectedParameterType = value; OnChanged(); UpdateBoxes(); PlotData(); } }

        private MaterialDto? _selectedMaterial;
        public MaterialDto? SelectedMaterial { get => _selectedMaterial; set { _selectedMaterial = value; OnChanged(); PlotData(); } }

        private string? _selectedBuildupCoefficient;
        public string? SelectedBuildupCoefficient { get => _selectedBuildupCoefficient; set { _selectedBuildupCoefficient = value; OnChanged(); PlotData(); } }

        public List<InterpolatedParameterType> AvailableParamaters { get; }

        private readonly double[] _energies;


        public ObservableCollection<MaterialDto> AvailableMaterials { get; }
        public List<string> AvailableBuildupCoefficients { get; }

        public bool IsMaterialsListEnabled { get => _isMaterialsListEnabled; set { _isMaterialsListEnabled = value; OnChanged(); } }
        public bool IsBuildupFactorsListEnabled { get => _isBuildupFactorsListEnabled; set { _isBuildupFactorsListEnabled = value; OnChanged(); } }

        public PlotModel PlotModel { get; private set; }
        #endregion

        #region CanExecute
        private bool CanExecute(object parameter)
        {
            if (_selectedParameterType != null)
            {
                IsMaterialsListEnabled = _selectedParameterType != InterpolatedParameterType.DoseConversionFactors;
                IsBuildupFactorsListEnabled = _selectedParameterType == InterpolatedParameterType.BuildupFactors;

                switch (_selectedParameterType)
                {
                    case InterpolatedParameterType.AbsorptionFactors:
                        return _selectedMaterial != null;

                    case InterpolatedParameterType.AttenuationFactors:
                        return _selectedMaterial != null;

                    case InterpolatedParameterType.BuildupFactors:
                        return _selectedMaterial != null && _selectedBuildupCoefficient != null;

                    case InterpolatedParameterType.DoseConversionFactors:
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region UpdateBoxes
        public void UpdateBoxes()
        {
            switch (_selectedParameterType)
            {
                case InterpolatedParameterType.AbsorptionFactors:
                    UpdateMaterialsList();
                    break;
                case InterpolatedParameterType.AttenuationFactors:
                    UpdateMaterialsList();
                    break;
                case InterpolatedParameterType.BuildupFactors:
                    UpdateMaterialsList();
                    break;
                case InterpolatedParameterType.DoseConversionFactors:
                    break;

            }
        }
        #endregion

        #region UpdateMaterialsList
        private void UpdateMaterialsList()
        {
            if (_selectedParameterType == InterpolatedParameterType.AbsorptionFactors)
            {
                AvailableMaterials.Clear();
                AvailableMaterials.Add(_environmentMaterial);
            }

            if (_selectedParameterType == InterpolatedParameterType.AttenuationFactors || _selectedParameterType == InterpolatedParameterType.BuildupFactors)
            {
                AvailableMaterials.Clear();
                AvailableMaterials.Add(_environmentMaterial);
                foreach (var material in _userMaterials)
                {
                    if (!AvailableMaterials.Select(m => m.Id).ToArray().Contains(material.Id))
                        AvailableMaterials.Add(material);
                }
            }
            SelectedMaterial = AvailableMaterials.FirstOrDefault();
        }
        #endregion

        #region PlotData
        public void PlotData()
        {
            if (_selectedParameterType == null || SelectedMaterial == null || SelectedBuildupCoefficient == null)
                return;

            ClearPlotData();

            switch (_selectedParameterType)
            {
                case InterpolatedParameterType.AbsorptionFactors:
                    PlotAbsorptionCoefficients();
                    break;
                case InterpolatedParameterType.AttenuationFactors:
                    PlotAttenuationCoefficients();
                    break;
                case InterpolatedParameterType.BuildupFactors:
                    PlotBuildupFactors();
                    break;
                case InterpolatedParameterType.DoseConversionFactors:
                    PlotDoseConversionFactors();
                    break;

            }
        }
        #endregion

        #region RequestDataToPlot
        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) RequestDataToPlot()
        {
            switch (_selectedParameterType)
            {
                case InterpolatedParameterType.AbsorptionFactors:
                    return ReturnAbsorptionData();

                case InterpolatedParameterType.AttenuationFactors:
                    return ReturnAttenuationData();

                case InterpolatedParameterType.BuildupFactors:
                    return ReturnBuildupData();

                case InterpolatedParameterType.DoseConversionFactors:
                    return ReturnDoseFactorsData();
            }
            return (new double[0], new double[0], new double[0], "");
        }

        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnAttenuationData()
        {
            (var table_energies, var table_values) = _materialsService.GetTableMassAttenuationFactors(_selectedMaterial!.Id);
            var interpolatedValues = _materialsService.GetInterpolatedMassAttenuationFactors(_selectedMaterial.Id, _energies);
            return (table_energies, table_values, interpolatedValues, $"Mass attenuation coefficients (cm²/g) for {_selectedMaterial.Name} ({SelectedMaterial!.Density} g/cm³)");
        }

        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnAbsorptionData()
        {
            (var table_energies, var table_values) = _materialsService.GetTableMassAbsoprtionFactors(_selectedMaterial!.Id);
            var interpolatedValues = _materialsService.GetInterpolatedMassAbsorptionFactors(_selectedMaterial.Id, _energies);
            return (table_energies, table_values, interpolatedValues, $"Mass absorption coefficients (cm²/g) for {_selectedMaterial.Name} ({SelectedMaterial!.Density} g/cm³)");
        }

        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnBuildupData()
        {
            var coefficientIndex = AvailableBuildupCoefficients.IndexOf(_selectedBuildupCoefficient);
            if (coefficientIndex < 0)
                return (new double[0], new double[0], new double[0], "");

            (var tableEnergies, var tableValuesArray) = _buildupService.GetTableFactors(_selectedBuildupType, _selectedMaterial!.Id);
            var tableValues = tableValuesArray.Select(v => v[coefficientIndex]).ToArray();

            var interpolatedValues = _buildupService.GetInterpolatedBuildupFactors(tableEnergies, tableValues, _energies);
            return (tableEnergies, tableValues, interpolatedValues, $"Buildup factor '{_selectedBuildupCoefficient}' for {_selectedMaterial.Name} ({SelectedMaterial!.Density} g/cm³)");
        }


        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnDoseFactorsData()
        {
            return (_tableDoseFactorsEnergies, _tableDoseFactorsValues, _interpolatedDoseFactors, $"{_doseFactorName} ({_doseFactorUnits})");
        }
        #endregion


        private void PlotAttenuationCoefficients()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnAttenuationData();
            PlotData(tableEnergies, tableValues, _energies, interpolatedData, title, isLogX: true, isLogY: true);
        }

        private void PlotAbsorptionCoefficients()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnAbsorptionData();
            PlotData(tableEnergies, tableValues, _energies, interpolatedData, title, isLogX: true, isLogY: true);
        }

        private void PlotDoseConversionFactors()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnDoseFactorsData();
            PlotData(tableEnergies, tableValues, _energies, interpolatedData, title, isLogX: true, isLogY: true);
        }

        private void PlotBuildupFactors()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnBuildupData();
            PlotData(tableEnergies, tableValues, _energies, interpolatedData, title, isLogX: true, isLogY: false);
        }

        private void ClearPlotData()
        {
            PlotModel.Axes.Clear();
            PlotModel.Series.Clear();
            PlotModel.Legends.Clear();
        }

        private void ResetScale()
        {
            PlotModel.ResetAllAxes();
            PlotModel.InvalidatePlot(false);
        }

        private void PlotData(double[] tableX, double[] tableY, double[] x, double[] y, string title = "", bool isLogX = false, bool isLogY = false)
        {
            PlotModel.Title = title;
            var minimumX = Math.Min(tableX.Min(), x.Min());
            var maximumX = Math.Max(tableX.Max(), x.Max());

            var minimumY = Math.Min(tableY.Min(), y.Min());
            var maximumY = Math.Max(tableY.Max(), y.Max());

            if (isLogX)
                PlotModel.Axes.Add(new LogarithmicAxis() { Title = "Energy, MeV", Position = AxisPosition.Bottom, Minimum = minimumX, Maximum = maximumX });
            else
                PlotModel.Axes.Add(new LinearAxis() { Title = "Energy, MeV", Position = AxisPosition.Bottom, Minimum = minimumX, Maximum = maximumX });

            if (isLogY)
                PlotModel.Axes.Add(new LogarithmicAxis() { Title = "Value", Position = AxisPosition.Left, Minimum = minimumY, Maximum = maximumY });
            else
                PlotModel.Axes.Add(new LinearAxis() { Title = "Value", Position = AxisPosition.Left, Minimum = minimumY, Maximum = maximumY });

            var tableSeries = new LineSeries() { Color = OxyColors.Blue, Title = "Table values", RenderInLegend = true };
            tableSeries.Points.AddRange(Enumerable.Range(0, tableX.Length).Select(i => new DataPoint(tableX[i], tableY[i])).ToArray());

            var userSeries = new ScatterSeries() { Title = "Interpolated values", RenderInLegend = true, MarkerFill = OxyColors.Red, MarkerType = MarkerType.Circle };
            userSeries.Points.AddRange(Enumerable.Range(0, x.Length).Select(i => new ScatterPoint(x[i], y[i], 3, tag: "o")).ToArray());

            PlotModel.Series.Add(tableSeries);
            PlotModel.Series.Add(userSeries);

            PlotModel.Legends.Add(new Legend()
            {
                LegendTextColor = OxyColors.Black,
                IsLegendVisible = true,
                LegendPosition = LegendPosition.TopRight,
                LegendPlacement = LegendPlacement.Inside
            });

            PlotModel.InvalidatePlot(true);
        }

        private void OpenTableView()
        {
            (var tableX, var tableY, var Y, string title) = RequestDataToPlot();
            new InterpolatedDataTableView(tableX, tableY, _energies, Y, title).Show();
        }

        private void ExportToExcel()
        {

        }
    }
}
