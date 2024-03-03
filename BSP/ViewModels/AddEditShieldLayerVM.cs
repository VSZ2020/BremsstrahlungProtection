using BSP.BL.DTO;
using BSP.BL.Materials;
using BSP.Common;
using System.Linq;
using System.Windows;

namespace BSP.ViewModels
{
    public class AddEditShieldLayerVM : BaseValidationViewModel
    {
        public AddEditShieldLayerVM(ShieldLayer layer = null)
        {
            if (layer != null)
            {
                SelectedMaterial = AvailableDataController.AvailableMaterials.SingleOrDefault(m => m.Id == layer.Id);
                Thickness = layer.D;
                Density = layer.Density;
            }
            else
            {
                SelectedMaterial = AvailableDataController.AvailableMaterials.FirstOrDefault();
                this.Layer = new ShieldLayer()
                {
                    Name = selectedMaterial?.Name ?? "",
                    Z = selectedMaterial?.Z ?? 0,
                    Density = selectedMaterial?.Density ?? 0,
                    Weight = selectedMaterial?.Weight ?? 0,
                };
            }

        }

        private float thickness = 0;
        private float density = 0;
        private MaterialDto selectedMaterial;

        public float Thickness { get => thickness; set { thickness = value; OnChanged(); } }
        public float Density { get => density; set { density = value; OnChanged(); } }
        public MaterialDto SelectedMaterial { get => selectedMaterial; set { selectedMaterial = value; OnChanged(); Density = selectedMaterial?.Density ?? 0; } }

        public ShieldLayer Layer { get; set; }

        public bool IsAppliedChanges { get; private set; } = false;


        private RelayCommand okCommand;
        public RelayCommand OkCommand => okCommand ?? (okCommand = new RelayCommand(obj => ApplyChanges(obj as Window)));


        private bool ValidateInputs()
        {
            base.ClearValidationMessages();
            if (selectedMaterial == null)
                base.AddError("Choose material");

            if (thickness <= 0)
                base.AddError("Thickness should be greater zero");

            if (density <= 0)
                base.AddError("Density should be greater zero");

            return IsValid;
        }

        public void ApplyChanges(Window w)
        {
            if (ValidateInputs())
            {
                Layer = new ShieldLayer()
                {
                    Id = selectedMaterial.Id,
                    Name = selectedMaterial.Name,
                    Z = selectedMaterial.Z,
                    Weight = selectedMaterial.Weight,
                    D = thickness,
                    Density = density,
                };

                IsAppliedChanges = true;
                w?.Close();
            }
        }
    }
}
