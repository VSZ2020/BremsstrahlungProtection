using BSP.BL.Materials;
using BSP.ViewModels;
using System.Windows;

namespace BSP.Views
{
    /// <summary>
    /// Логика взаимодействия для AddEditShieldLayerViewModel.xaml
    /// </summary>
    public partial class AddEditShieldLayerWindow : Window
    {
        private AddEditShieldLayerVM vm;

        public AddEditShieldLayerWindow(ShieldLayer layer = null)
        {
            vm = new AddEditShieldLayerVM(layer);
            DataContext = vm;
            InitializeComponent();
        }

        public ShieldLayer Layer => vm.Layer;
        public bool IsAppliedChanges => vm.IsAppliedChanges;

    }
}
