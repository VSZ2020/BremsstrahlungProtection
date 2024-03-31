using BSP.BL.Services;
using BSP.Common;
using BSP.FileUtils;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace BSP.ViewModels.RadionuclidesViewer
{
    public class RadionuclidesViewerVM: BaseViewModel
    {
        #region Constructor
        public RadionuclidesViewerVM(RadionuclidesService radionuclidesService)
        {
            this._service = radionuclidesService;
            var radionuclides = radionuclidesService.GetAllRadionuclides();
            RadionuclidesList = radionuclides.Select(r => new TableRadionuclideVM() { Id = r.Id, Name = r.Name, HalfLive = r.HalfLive, HalfLiveUnits = r.HalfLiveUnits }).ToList();

            EnergyIntensityList = new();
            SelectedRadionuclide = RadionuclidesList.FirstOrDefault();
        }
        #endregion

        #region Fields
        private readonly RadionuclidesService _service;

        private TableRadionuclideVM? _selectedRadionuclide;
        private RadionuclideEnergyIntensityVM? _selectedEnergyIntensity;
        #endregion

        #region Properties
        public TableRadionuclideVM? SelectedRadionuclide { get => _selectedRadionuclide; set { _selectedRadionuclide = value; OnChanged(); UpdateEnergyIntensityList(); } }
        public RadionuclideEnergyIntensityVM? SelectedEnergyIntensity { get => _selectedEnergyIntensity; set { _selectedEnergyIntensity = value; OnChanged(); } }

        public List<TableRadionuclideVM> RadionuclidesList { get; }
        public ObservableCollection<RadionuclideEnergyIntensityVM> EnergyIntensityList { get; } 
        #endregion

        #region Commands
        private RelayCommand exportCommand;
        public RelayCommand ExportCommand => exportCommand ?? (exportCommand = new RelayCommand(o => Export(), o => _selectedRadionuclide != null));
        #endregion

        #region UpdateEnergyIntensityList
        private void UpdateEnergyIntensityList()
        {
            if (_selectedRadionuclide == null)
                return;

            ClearEnergyIntensityData();
            var radionclideEnergyIntensity = _service.GetEnergyIntensityData(_selectedRadionuclide.Id).Select(r => new RadionuclideEnergyIntensityVM()
            {
                EndpointEnergy = r.EndpointEnergy,
                AverageEnergy = r.AverageEnergy,
                Intensity = r.Yield,
            }).ToList();

            //Search line with max energy-intensity
            var line = radionclideEnergyIntensity.MaxBy(i => i.EndpointEnergy * i.Intensity);
            if (line != null)
                line.IsMajorLine = true;

            foreach (var item in radionclideEnergyIntensity)
                EnergyIntensityList.Add(item);

        }
        #endregion

        #region ClearEnergyIntensityData
        private void ClearEnergyIntensityData()
        {
            EnergyIntensityList.Clear();
        } 
        #endregion

        #region Export
        private void Export()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text File (*.txt)|*.txt|Comma-separated values (*.csv)|*.csv";
            saveDialog.FilterIndex = 0;
            saveDialog.AddExtension = true;

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                if (FileService.ExportRadionuclideEnergyIntensity(saveDialog.FileName, EnergyIntensityList.ToList()))
                    MessageBox.Show($"Successfully saved at {saveDialog.FileName}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        } 
        #endregion
    }
}
