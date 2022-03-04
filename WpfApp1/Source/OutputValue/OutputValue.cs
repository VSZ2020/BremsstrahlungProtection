using System;
namespace BSP
{
    public class OutputValue
    {

		/// <summary>
		/// Возвращает суммарную мощность дозы
		/// </summary>
		public double DoseRate
		{
			get
			{
				double bufDoseRate = 0;
				for (int i = 0; i < DoseRatePart.Length; i++)
					bufDoseRate += DoseRatePart[i];
				return bufDoseRate;
			}
		}
		/// <summary>
		/// Массив с парциальными мощностями доз
		/// </summary>
		public double[] DoseRatePart;

	}
}
