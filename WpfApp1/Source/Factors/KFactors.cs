using System;


namespace BSP
{
	/// <summary>
	/// Класс с коэф-тами для расчета фактора накопления по формуле Японцев
	/// </summary>
	public class KFactors
	{
		public double[] Energy;
		public double[] a;
		public double[] b;
		public double[] c;
		public double[] d;
		public double[] xk;

		/// <summary>
		/// Конструктор класса KFactors
		/// </summary>
		/// <param name="Count">Количество энергий излучения</param>
		public KFactors(int Count)
		{
			Energy = new double[Count];
			a = new double[Count];
			b = new double[Count];
			c = new double[Count];
			d = new double[Count];
			xk = new double[Count];
		}
	}
}
