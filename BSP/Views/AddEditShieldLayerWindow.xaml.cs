using BSP.ViewModels;
using System.Windows;
using BSP.Geometries.SDK;

namespace BSP.Views
{
    /// <summary>
    /// Логика взаимодействия для AddEditShieldLayerViewModel.xaml
    /// </summary>
    public partial class AddEditShieldLayerWindow : Window
    {
        private AddEditShieldLayerVM vm;

        public AddEditShieldLayerWindow(ShieldLayerVM? layer = null)
        {
            vm = new AddEditShieldLayerVM(layer);
            DataContext = vm;
            InitializeComponent();
            this.Title = layer == null ? "Add new shield layer" : $"Edit shield layer {layer.Name}";
        }

        public ShieldLayerVM? Layer => vm.Layer;
        public bool IsAppliedChanges => vm.IsAppliedChanges;

    }
}
