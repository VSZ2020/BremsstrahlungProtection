using System.Windows;

namespace BSP.Source.Calculation.CalcDirections
{
	public class YCalcDirection : ADirection
	{
		public override CalculationDirection Direction => CalculationDirection.Y;

		public override string Name => (string)Application.Current.Resources["RadDirectionY"];
	}
}
