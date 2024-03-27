using BSP.BL.DTO;
using BSP.Common;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using BSP.Geometries.SDK;

namespace BSP.ViewModels
{
    public class AddEditShieldLayerVM : BaseValidationViewModel, IDataErrorInfo
    {
        public AddEditShieldLayerVM(ShieldLayerVM? layer = null)
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
                this.Layer = new ShieldLayerVM()
                {
                    Name = selectedMaterial?.Name ?? "",
                    Z = selectedMaterial?.Z ?? 0,
                    Density = selectedMaterial?.Density ?? 0,
                    Weight = selectedMaterial?.Weight ?? 0,
                };
            }

        }

        private float thickness = 1;
        private float density = 1;
        private MaterialDto? selectedMaterial;

        public float Thickness { get => thickness; set { thickness = value; OnChanged(); } }
        public float Density { get => density; set { density = value; OnChanged(); } }
        public MaterialDto? SelectedMaterial { get => selectedMaterial; set { selectedMaterial = value; OnChanged(); Density = selectedMaterial?.Density ?? 0; } }

        public ShieldLayerVM? Layer { get; set; }

        public bool IsAppliedChanges { get; private set; } = false;


        private RelayCommand okCommand;
        public RelayCommand OkCommand => okCommand ?? (okCommand = new RelayCommand(obj => ApplyChanges(obj as Window)));


        private bool ValidateInputs()
        {
            base.ClearValidationMessages();
            if (selectedMaterial == null)
                base.AddError("Choose material");

            if (thickness <= 0)
                base.AddError((Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Incorrect value");

            if (density <= 0)
                base.AddError((Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Incorrect value");

            return IsValid;
        }

        public void ApplyChanges(Window w)
        {
            if (ValidateInputs())
            {
                Layer = new ShieldLayerVM()
                {
                    Id = selectedMaterial!.Id,
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

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Thickness):
                        if (Thickness <= 0)
                            error =(Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Incorrect value";
                        break;
                    case nameof(Density):
                        if (Density <= 0)
                            error = (Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Incorrect value";
                        break;
                }

                return error;
            }
        }
        public string Error => throw new NotImplementedException();
    }
}
