using System.Windows;

namespace BSP.Source.Calculation.CalcDirections
{
	public class XCalcDirection : ADirection
	{
		public override CalculationDirection Direction => CalculationDirection.X;

		public override string Name => (string)Application.Current.Resources["RadDirectionX"];
	}
}
