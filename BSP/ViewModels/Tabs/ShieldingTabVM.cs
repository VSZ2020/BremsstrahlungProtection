using BSP.Common;
using BSP.Views;
using System.Collections.ObjectModel;

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
        RelayCommand moveUpCommand;
        RelayCommand moveDownCommand;

        public RelayCommand AddCommand => addCommand ?? (addCommand = new RelayCommand(obj => AddShieldLayer(), o => hasMaterials));
        public RelayCommand EditCommand => editCommand ?? (editCommand = new RelayCommand(obj => EditShieldLayer(), o => hasMaterials && _selectedShieldLayer != null));
        public RelayCommand RemoveCommand => removeCommand ?? (removeCommand = new RelayCommand(obj => RemoveShieldLayer(), o => hasMaterials && _selectedShieldLayer != null));
        public RelayCommand MoveUpCommand => moveUpCommand ?? (moveUpCommand = new RelayCommand(o => MoveShieldUp(), o => _selectedShieldLayer != null && ShieldLayers.IndexOf(_selectedShieldLayer) > 0));
        public RelayCommand MoveDownCommand => moveDownCommand ?? (moveDownCommand = new RelayCommand(o => MoveShieldDown(), o => _selectedShieldLayer != null && ShieldLayers.IndexOf(_selectedShieldLayer) < ShieldLayers.Count - 1));
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

                SelectedShieldLayer = ShieldLayers.LastOrDefault(l => l.Id == addWnd.Layer.Id);
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

        private void MoveShieldUp()
        {
            var curIndex = ShieldLayers.IndexOf(_selectedShieldLayer);
            ShieldLayers.Move(curIndex, curIndex - 1);
        }

        private void MoveShieldDown()
        {
            var curIndex = ShieldLayers.IndexOf(_selectedShieldLayer);
            ShieldLayers.Move(curIndex, curIndex + 1);
        }
    }
}
