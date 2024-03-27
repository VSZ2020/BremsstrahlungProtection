using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using BSP.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BSP.Geometries.SDK;

namespace BSP.ViewModels.Tabs
{
    public class ShieldingTabVM : BaseViewModel
    {
        public ShieldingTabVM()
        {
            ShieldLayers = new();
        }

        private bool hasMaterials => AvailableDataController.AvailableMaterials.Count > 0;
        private ShieldLayerVM? _selectedShieldLayer;
        
        public ObservableCollection<ShieldLayerVM> ShieldLayers { get; }
        public ShieldLayerVM? SelectedShieldLayer { get => _selectedShieldLayer; set { _selectedShieldLayer = value; OnChanged(); } }


        #region Commands
        RelayCommand addCommand;
        RelayCommand editCommand;
        RelayCommand removeCommand;

        public RelayCommand AddCommand => addCommand ?? (addCommand = new RelayCommand(obj => AddShieldLayer(), o => hasMaterials));
        public RelayCommand EditCommand => editCommand ?? (editCommand = new RelayCommand(obj => EditShieldLayer(), o => hasMaterials && _selectedShieldLayer != null));
        public RelayCommand RemoveCommand => removeCommand ?? (removeCommand = new RelayCommand(obj => RemoveShieldLayer(), o => hasMaterials && _selectedShieldLayer != null));
        #endregion

        public void AddShieldLayer()
        {
            //var shieldLayer = new ShieldLayer()
            //{
            //    Id = AvailableMaterials.First().Id,
            //    Name = AvailableMaterials.First().Name,
            //    D = 0,
            //    Density = AvailableMaterials.First().Density,
            //};

            //if (SelectedShieldLayer != null)
            //{
            //    var index = ShieldLayers.IndexOf(_selectedShieldLayer);
            //    ShieldLayers.Insert(index + 1, shieldLayer);
            //}
            //else
            //    ShieldLayers.Add(shieldLayer);
            var addWnd = new AddEditShieldLayerWindow();
            addWnd.ShowDialog();
            if (addWnd.IsAppliedChanges)
            {
                if (SelectedShieldLayer != null)
                {
                    var index = ShieldLayers.IndexOf(_selectedShieldLayer);
                    ShieldLayers.Insert(index + 1, addWnd.Layer);
                }
                else
                    ShieldLayers.Add(addWnd.Layer);

                SelectedShieldLayer = ShieldLayers.SingleOrDefault(l => l.Id == addWnd.Layer.Id);
            }
        }

        public void RemoveShieldLayer()
        {
            var index = ShieldLayers.IndexOf(_selectedShieldLayer);
            ShieldLayers.Remove(_selectedShieldLayer);

            if (index > 0 && index < ShieldLayers.Count)
                SelectedShieldLayer = ShieldLayers[index - 1];
            else if (ShieldLayers.Count > 0)
                SelectedShieldLayer = ShieldLayers[0];
        }

        public void EditShieldLayer()
        {
            var editWnd = new AddEditShieldLayerWindow(_selectedShieldLayer);
            editWnd.ShowDialog();
            if (editWnd.IsAppliedChanges)
            {
                int index = ShieldLayers.IndexOf(_selectedShieldLayer);
                ShieldLayers[index] = editWnd.Layer;
            }
        }
    }
}
