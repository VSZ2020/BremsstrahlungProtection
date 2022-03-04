
namespace BSP
{
	public class FactorValue
	{
		public double[] Energy;
		public double[] Value;

		public FactorValue(int EnergiesCount)
		{
			Energy = new double[EnergiesCount];
			Value = new double[EnergiesCount];
		}
		public FactorValue()
		{
			Energy = new double[5];
			Value = new double[5];

			for (int i = 0; i < 5; i++)
			{
				Energy[i] = i+1;
				Value[i] = 1;
			}
		}
	}
}
