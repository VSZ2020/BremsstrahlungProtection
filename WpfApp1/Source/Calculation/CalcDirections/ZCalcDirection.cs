using System.Windows;

namespace BSP.Source.Calculation.CalcDirections
{
	public class ZCalcDirection : ADirection
	{
		public override CalculationDirection Direction => CalculationDirection.Z;

		public override string Name => (string)Application.Current.Resources["RadDirectionZ"];
	}
}
