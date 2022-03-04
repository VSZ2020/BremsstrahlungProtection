
namespace BSP
{
	public class TaylorFactors
	{
		public double[] Energy;
		public double[] A1;
		public double[] a1;
		public double[] a2;
		public double[] Delta;

		public TaylorFactors(int Count)
		{
			Energy = new double[Count];
			A1 = new double[Count];
			a1 = new double[Count];
			a2 = new double[Count];
			Delta = new double[Count];
		}
	}
}
