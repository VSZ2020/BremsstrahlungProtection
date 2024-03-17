using BSP.Common;
using System.ComponentModel;
using System.Windows;

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
        public float Value { get { return this.value; } set { this.value = value; OnChanged(); } }
        public int Discreteness { get { return discreteness; } set { discreteness = value; OnChanged(); } }
        public bool IsValueAvailable { get => isValueAvailable; set { isValueAvailable = value; OnChanged(); } }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Value):
                        if (Value <= 0)
                            error = string.Format((Application.Current.TryFindResource("msg_ValidationDimension") as string)?? "Incorrect value", name);
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
