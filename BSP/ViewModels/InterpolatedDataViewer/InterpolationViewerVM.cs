using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using BSP.Views;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace BSP.ViewModels.InterpolatedDataViewer
{
    /// <summary>
    /// Пример плохого кода, но это работает
    /// </summary>
    public class InterpolationViewerVM: BaseViewModel
    {
        public InterpolationViewerVM(double[] bremsstrahlungEnergies, int selectedEnvironmentMaterialId, int[] selectedMaterialsIds, Type selectedBuildupType, MaterialsService materialsService, BuildupService buildupService, DoseFactorsService doseFactorsService, Type selectedDoseFactorType, int exposureGeometryId, int OrganTissueId)
        {
            this.energies = bremsstrahlungEnergies;
            this.selectedBuildupType = selectedBuildupType;

            this.materialsService = materialsService;
            this.buildupService = buildupService;

            environmentMaterial = materialsService.GetMaterialById(selectedEnvironmentMaterialId);
            userMaterials = materialsService.GetMaterialsById(selectedMaterialsIds);
            AvailableMaterials = new ObservableCollection<MaterialDto>();

            AvailableBuildupCoefficients = new(BuildupService.GetBuildupCoefficientsNames(selectedBuildupType));

            AvailableParamaters = new List<InterpolatedParameterType>()
            {
                InterpolatedParameterType.AttenuationFactors,
                InterpolatedParameterType.AbsorptionFactors,
                InterpolatedParameterType.BuildupFactors,
                InterpolatedParameterType.DoseConversionFactors
            };
            SelectedParameterType = AvailableParamaters.FirstOrDefault();
            SelectedMaterial = userMaterials.FirstOrDefault();
            SelectedBuildupCoefficient = AvailableBuildupCoefficients.FirstOrDefault();

            //Заранее интерполируем и заполняем данные по коэффициентам конверсии
            (tableDoseFactorsEnergies, tableDoseFactorsValues) = doseFactorsService.GetTableDoseConversionFactors(selectedDoseFactorType, exposureGeometryId, OrganTissueId);
            interpolatedDoseFactors = doseFactorsService.GetDoseConversionFactors(selectedDoseFactorType, bremsstrahlungEnergies, exposureGeometryId, OrganTissueId);
            var doseFactorTranslation = Application.Current.TryFindResource(DoseFactorsService.GetTranslationKey(selectedDoseFactorType));
            DoseFactorName = doseFactorTranslation != null ? (string)doseFactorTranslation: "Dose conversion factor";
            doseFactorUnits = DoseFactorsService.GetDoseConversionFactorUnits(selectedDoseFactorType);

            ShowData();
        }

        #region Commands
        private RelayCommand showCommand;
        public RelayCommand ShowCommand => showCommand ?? (showCommand = new RelayCommand(o => ShowData(), CanExecute));

        private RelayCommand exportToExcel;
        public RelayCommand ExportToExcelCommand => exportToExcel ?? (exportToExcel = new RelayCommand(o => ExportToExcel(), o => false));

        private RelayCommand openTableView;
        public RelayCommand OpenTableViewCommand => openTableView ?? (openTableView = new RelayCommand(o => OpenTableView(), CanExecute));
        #endregion

        #region Properties and fields
        private readonly MaterialsService materialsService;
        private readonly BuildupService buildupService;

        public string DoseFactorName { get; set; }
        private string doseFactorUnits;
        private double[] tableDoseFactorsEnergies;
        private double[] tableDoseFactorsValues;
        private double[] interpolatedDoseFactors;

        private Type selectedBuildupType;
        private List<MaterialDto> userMaterials;
        private MaterialDto environmentMaterial;


        private InterpolatedParameterType? selectedParameterType;
        public InterpolatedParameterType? SelectedParameterType { get => selectedParameterType; set { selectedParameterType = value; OnChanged(); UpdateBoxes();  } }

        private MaterialDto? selectedMaterial;
        public MaterialDto? SelectedMaterial { get => selectedMaterial; set { selectedMaterial = value; OnChanged(); } }

        private string selectedBuildupCoefficient;
        public string SelectedBuildupCoefficient { get => selectedBuildupCoefficient; set { selectedBuildupCoefficient = value; OnChanged(); } }

        public List<InterpolatedParameterType> AvailableParamaters { get; }

        private readonly double[] energies;
        

        public ObservableCollection<MaterialDto> AvailableMaterials { get; }
        public List<string> AvailableBuildupCoefficients { get; }

        public PlotModel PlotModel { get; private set; } = new();
        #endregion

        private bool CanExecute(object parameter)
        {
            if (selectedParameterType != null)
            {
                switch (selectedParameterType)
                {
                    case InterpolatedParameterType.AbsorptionFactors:
                        return selectedMaterial != null;
                        
                    case InterpolatedParameterType.AttenuationFactors:
                        return selectedMaterial != null;

                    case InterpolatedParameterType.BuildupFactors:
                        return selectedMaterial != null && selectedBuildupCoefficient != null;

                    case InterpolatedParameterType.DoseConversionFactors:
                        return true;
                }
            }
            
            return false;
        }

        public void UpdateBoxes()
        {
            switch (selectedParameterType)
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

        private void UpdateMaterialsList()
        {
            if (selectedParameterType == InterpolatedParameterType.AbsorptionFactors)
            {
                AvailableMaterials.Clear();
                AvailableMaterials.Add(environmentMaterial);
            }

            if (selectedParameterType == InterpolatedParameterType.AttenuationFactors || selectedParameterType == InterpolatedParameterType.BuildupFactors)
            {
                AvailableMaterials.Clear();
                foreach(var material in userMaterials)
                {
                    AvailableMaterials.Add(material);
                }
            }
            SelectedMaterial = AvailableMaterials.FirstOrDefault();
        }

        public void ShowData()
        {
            ClearPlotData();

            switch (selectedParameterType)
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

        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) RequestDataToPlot()
        {
            switch (selectedParameterType)
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
            (var table_energies, var table_values) = materialsService.GetTableMassAttenuationFactors(selectedMaterial.Id);
            var interpolatedValues = materialsService.GetInterpolatedMassAttenuationFactors(selectedMaterial.Id, energies);
            return (table_energies, table_values, interpolatedValues, $"Attenuation coefficients for {selectedMaterial.Name}");
        }

        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnAbsorptionData()
        {
            (var table_energies, var table_values) = materialsService.GetTableMassAbsoprtionFactors(selectedMaterial.Id);
            var interpolatedValues = materialsService.GetInterpolatedMassAbsorptionFactors(selectedMaterial.Id, energies);
            return (table_energies, table_values, interpolatedValues, $"Mass absorption coefficients for {selectedMaterial.Name}");
        }

        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnBuildupData()
        {
            var coefficientIndex = AvailableBuildupCoefficients.IndexOf(selectedBuildupCoefficient);
            if (coefficientIndex < 0)
                return (new double[0], new double[0], new double[0], "");

            (var tableEnergies, var tableValuesArray) = buildupService.GetTableFactors(selectedBuildupType, selectedMaterial.Id);
            var tableValues = tableValuesArray.Select(v => v[coefficientIndex]).ToArray();

            var interpolatedValues = buildupService.GetInterpolatedBuildupFactors(tableEnergies, tableValues, energies);
            return (tableEnergies, tableValues, interpolatedValues, $"Buildup factor '{selectedBuildupCoefficient}' for {selectedMaterial.Name}");
        }


        private (double[] tableX, double[] tableY, double[] interpolatedY, string header) ReturnDoseFactorsData()
        {
            return (tableDoseFactorsEnergies, tableDoseFactorsValues, interpolatedDoseFactors, $"{DoseFactorName} ({doseFactorUnits})");
        }


        private void PlotAttenuationCoefficients()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnAttenuationData();
            PlotData(tableEnergies, tableValues, energies, interpolatedData, title, isLogX: true, isLogY: true);
        }

        private void PlotAbsorptionCoefficients()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnAbsorptionData();
            PlotData(tableEnergies, tableValues, energies, interpolatedData, title, isLogX: true, isLogY: true);
        }

        private void PlotDoseConversionFactors()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnDoseFactorsData();
            PlotData(tableEnergies, tableValues, energies, interpolatedData, title, isLogX: true, isLogY: true);
        }

        private void PlotBuildupFactors()
        {
            (var tableEnergies, var tableValues, var interpolatedData, var title) = ReturnBuildupData();
            PlotData(tableEnergies, tableValues, energies, interpolatedData, title, isLogX: true, isLogY: false);
        }

        private void ClearPlotData()
        {
            PlotModel.Axes.Clear();
            PlotModel.Series.Clear();
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

            var tableSeries = new LineSeries() { Color = OxyColors.Blue, SeriesGroupName = "Table values" };
            tableSeries.Points.AddRange(Enumerable.Range(0, tableX.Length).Select(i => new DataPoint(tableX[i], tableY[i])).ToArray());

            var userSeries = new ScatterSeries() { SeriesGroupName = "Interpolated values", MarkerFill = OxyColors.Red, MarkerType = MarkerType.Circle };
            userSeries.Points.AddRange(Enumerable.Range(0, x.Length).Select(i => new ScatterPoint(x[i], y[i], 3, tag:"o")).ToArray());

            PlotModel.Series.Add(tableSeries);
            PlotModel.Series.Add(userSeries);

            PlotModel.InvalidatePlot(false);
        }

        private void OpenTableView()
        {
            (var tableX, var tableY, var Y, string title) = RequestDataToPlot();
            new InterpolatedDataTableView(tableX, tableY, energies, Y, title).ShowDialog();
        }

        private void ExportToExcel()
        {

        }
    }
}
