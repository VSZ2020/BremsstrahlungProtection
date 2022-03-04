using System;

namespace BSP
{
	public class LSpline: ASpline
	{
		struct LinearSpline
		{
			public double a;
			public double b;
			public double tableX;
			public double tableY;
		}

		LinearSpline[] linSpline;
		private int n = 0;

		public LSpline(double[] x, double[] y)
		{
			if (x.Length != y.Length) throw new Exception("Размеры массивов X и Y различны");
			n = x.Length;
			linSpline = new LinearSpline[n];

			for (int i = 0; i < n; i++)
			{
				linSpline[i] = new LinearSpline();
				linSpline[i].tableX = x[i];
				linSpline[i].tableY = y[i];
			}
			

			for (int i = 0; i < n - 1; i++)
			{
				if (x[i] == x[i + 1])
					linSpline[i].a = 0;
				else
					linSpline[i].a = (y[i + 1] - y[i]) / (x[i + 1] - x[i]);
				linSpline[i].b = y[i] - linSpline[i].a * x[i];
			}

		}
		/// <summary>
		/// Возвращает интерполированное значение
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public override double GetValue(double x, float ENERGY_EDGE)
		{
			LinearSpline ls = new LinearSpline();
			if (x <= linSpline[0].tableX) ls = linSpline[0];
			else
				if (x >= linSpline[n - 1].tableX) ls = linSpline[n - 2];
			else
			{
				for (int i = 0; i < n - 1; i++)
				{
					if (x > linSpline[i].tableX && x < linSpline[i + 1].tableX)
					{
						if (x < ENERGY_EDGE)
						{
							//Если x ближе к правой точке, то ...
							if (Math.Abs(x - linSpline[i].tableX) > Math.Abs(x - linSpline[i + 1].tableX))
							{
								return linSpline[i + 1].tableY;
							}
							else if (Math.Abs(x - linSpline[i].tableX) < Math.Abs(x - linSpline[i + 1].tableX))
							{
								return linSpline[i].tableY;
							}
							else
								return linSpline[i].tableY;
						}
						else
							ls = linSpline[i];
						break;
					}

				}
			}
			return ls.a * x + ls.b;
		}

		public double GetValue2(double x)
		{
			LinearSpline ls = new LinearSpline();
			if (x <= linSpline[0].tableX) ls = linSpline[0];
			else
				if (x >= linSpline[n - 1].tableX) ls = linSpline[n - 2];
			else
			{
				for (int i = 0; i < n - 1; i++)
				{
					if (x > linSpline[i].tableX && x < linSpline[i + 1].tableX)
					{
						ls = linSpline[i];
						break;
					}

				}
			}
			return ls.a * x + ls.b;
		}

		public override double[] GetArray(double[] x, ref Material material)
		{
			double[] y = new double[x.Length];
			for (int i = 0; i < x.Length; i++)
				y[i] = GetValue(x[i], material.EnergyLimit);
			return y;
		}
	}
}
