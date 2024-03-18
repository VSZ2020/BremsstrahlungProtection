using BSP.Common;

namespace BSP.ViewModels.TableViewer
{
    public class InterpolatedDataTableViewerVM: BaseViewModel
    {
        public InterpolatedDataTableViewerVM(double[] tableX, double[] tableY, double[] X, double[] Y)
        {
            for(var i = 0; i < tableX.Length; i++)
            {
                TableValues.Add(new EnergyValuePair() { Energy = tableX[i], Value = tableY[i] });
            }

            for (var i = 0; i < X.Length; i++)
            {
                InterpolatedValues.Add(new EnergyValuePair(){ Energy = X[i], Value = Y[i]});
            }
        }

        public List<EnergyValuePair> TableValues { get; } = new();

        public List<EnergyValuePair> InterpolatedValues { get; } = new();

        public class EnergyValuePair
        {
            public double Value { get; set; }
            public double Energy { get; set; }
        }
    }
}
