using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BSP
{
	public static class DataReader
	{
		/// <summary>
		/// Расчет парциальных потоков энергии ТИ
		/// </summary>
		/// <param name="OutputField"></param>
		public static double[] CalculatePartialIBeta(ref TextBox OutputField)
		{
			try
			{
				int EnergiesCount = Breamsstrahlung.Length;
				double Ibmax = 0.0;                                                                                         //Переменная для вывода суммарного значения потока энергии
				double[] Ib = Breamsstrahlung.GetEnergyFlows(CalcParams.Source.Radionuclides, CalcParams.Source.Z);         //Передаем набор выбранных нуклидов на обработку
				for (int i = 0; i < EnergiesCount; i++)
					Ibmax += Ib[i];

				//Выводим Ibeta_max в поле на форме
				OutputField.Text = Math.Round(Ibmax, CalcParams.precision).ToString();

				return Ib;
			}
			catch { throw; }
			
		}
	}
}
