using BSP.BL.Analysis.Sensitivity;
using BSP.Common;
using System.ComponentModel;
using System.Windows;

namespace BSP.ViewModels
{
    public class DosePointVM : BaseViewModel, IDataErrorInfo, IVariableClass
    {
        private float x;
        private float y;
        private float z;

        [VariableParameter(MinValue = 0, Step = 1)]
        public float X { get => x; set { x = value; OnChanged(); } }

        [VariableParameter(MinValue = 0, Step = 1)]
        public float Y { get => y; set { y = value; OnChanged(); } }

        [VariableParameter(MinValue = 0, Step = 1)]
        public float Z { get => z; set { z = value; OnChanged(); } }

        public float[] Values => [X, Y, Z];

        public string Error => "";

        public VariableValueInfo[] VariableValues => [
            new VariableValueInfo() {
                Name = "X",
                MinBoundary = 0,
                From = 0,
                To = 100,
                Step = 0.1
            },
            new VariableValueInfo(){
                Name = "Y",
                MinBoundary = 0,
                From = 0,
                To = 100,
                Step = 0.1
            },
            new VariableValueInfo(){
                Name = "Z",
                MinBoundary = 0,
                From = 0,
                To = 100,
                Step = 0.1
            },
        ];

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
