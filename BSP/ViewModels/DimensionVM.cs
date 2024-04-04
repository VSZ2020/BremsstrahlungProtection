using BSP.BL.Analysis.Sensitivity;
using BSP.Common;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace BSP.ViewModels
{
    /// <summary>
    /// Модель, описывающая размер источника
    /// </summary>
    public class DimensionVM : BaseViewModel, IDataErrorInfo
    {
        private string? name;
        private float value = 10;
        private int discreteness = 10;
        private bool isValueAvailable = true;

        public string? Name { get { return name; } set { name = value; OnChanged(); } }

        [VariableParameter(MinValue = 0, Step = 0.5)]
        public float Value { get { return this.value; } set { this.value = value > 0 ? value : 1; OnChanged(); } }

        [VariableParameter(MinValue = 1, Step = 1)]
        public int Discreteness { get { return discreteness; } set { discreteness = value > 0 ? value : 1; OnChanged(); } }

        public bool IsValueAvailable { get => isValueAvailable; set { isValueAvailable = value; OnChanged(); } }

        public Brush ValueColor { get; set; } = Brushes.Black;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Value):
                        if (Value <= 0)
                            error = string.Format((Application.Current.TryFindResource("msg_ValidationDimension") as string) ?? "Incorrect value", name);
                        break;
                    case nameof(Discreteness):
                        if (discreteness <= 0)
                            error = string.Format((Application.Current.TryFindResource("msg_ValidationDimension") as string) ?? "Incorrect value", name);
                        break;
                }

                return error;
            }
        }
        public string Error => string.Empty;

    }
}
