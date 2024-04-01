using BSP.Common;
using System.ComponentModel;
using System.Windows;

namespace BSP.ViewModels
{
    public class DosePointVM : BaseViewModel, IDataErrorInfo
    {
        private float x;
        private float y;
        private float z;


        public float X { get => x; set { x = value; OnChanged(); } }
        public float Y { get => y; set { y = value; OnChanged(); } }
        public float Z { get => z; set { z = value; OnChanged(); } }

        public float[] Values => [X, Y, Z];

        public string Error => "";

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(X):
                    case nameof(Y):
                    case nameof(Z):
                        if (X <= 0 || Y < 0 || Z < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationGreaterZero") as string) ?? "Coordinate must be greater zero"; ;
                        break;
                }
                return error;
            }
        }
    }
}
