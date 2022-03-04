using System;


namespace BSP
{
	public class InterpolatedTaylor
	{
		public double[] A1;
		public double[] a1;
		public double[] a2;
		public double[] Delta;

		public InterpolatedTaylor(int EnergiesCount)
		{
			A1 = new double[EnergiesCount];
			a1 = new double[EnergiesCount];
			a2 = new double[EnergiesCount];
			Delta = new double[EnergiesCount];
		}
	}
}
