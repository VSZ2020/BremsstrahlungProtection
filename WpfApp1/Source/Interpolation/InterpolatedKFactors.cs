

namespace BSP
{
	public class InterpolatedKFactors
	{
		public double[] a;
		public double[] b;
		public double[] c;
		public double[] d;
		public double[] xk;

		public InterpolatedKFactors(int EnergiesCount)
		{
			a = new double[EnergiesCount];
			b = new double[EnergiesCount];
			c = new double[EnergiesCount];
			d = new double[EnergiesCount];
			xk = new double[EnergiesCount];
		}
	}
}
