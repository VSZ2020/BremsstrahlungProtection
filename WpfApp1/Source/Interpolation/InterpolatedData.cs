/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 08-Mar-20
 * Время: 22:07
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System.IO;

namespace BSP
{
	/// <summary>
	/// Хранит интерполированные данные
	/// </summary>
	public class InterpolatedData
	{
		/// <summary>
		/// Набор энергий
		/// </summary>
		public double[] Energy;

		/// <summary>
		/// Интерполированные значения дозового коэффициента
		/// </summary>
		public double[] DoseFactor;

		/// <summary>
		/// Интерполированные значения коэф.поглощения в воздухе
		/// </summary>
		public double[] um_absorbtion_air;

		/// <summary>
		/// Интерполированные значения для материалов защиты
		/// </summary>
		public InterpolatedMaterial[] MaterialData;

		/// <summary>
		/// Конструктор класса для хранения интерполированных значений
		/// </summary>
		/// <param name="EnergiesCount">Количество линий излучения</param>
		/// <param name="LayersCount">Количество слоев защиты</param>
		public InterpolatedData(int EnergiesCount, int LayersCount)
		{
			Energy = new double[EnergiesCount];
			DoseFactor = new double[EnergiesCount];
			MaterialData = new InterpolatedMaterial[LayersCount];
			for (int i = 0; i < LayersCount; i++)
			{
				MaterialData[i] = new InterpolatedMaterial(EnergiesCount);
			}
		}
		/*
		public void ExportDataToFile(string fileName)
		{
			using (StreamWriter sw = new StreamWriter("Env_"+fileName))
			{
				sw.WriteLine("E,MeV\tum (Air),cm2/g\tDoseFactor");
				for (int i = 0; i < DoseFactor.Length; i++)
				{
					sw.WriteLine(FactorsHandler.Energy[i] + "\t" + um_absorbtion_air[i] + "\t" + DoseFactor[i]);
				}
			}

			using (StreamWriter sw = new StreamWriter("Source_" + fileName))
			{
				sw.WriteLine("For radiation source");
				sw.WriteLine("E,Mev\tu,cm-1\tfactor a\tfactor b\tfactor c\tfactor d\tfactor xi");
				for (int i = 0; i < MaterialData[0].um_Absorbtion.Length; i++)
				{
					sw.WriteLine(Energy[i] + "\t" + MaterialData[0].um_Attenuation[i] + "\t" + a[0][i] + "\t" + b[0][i] + "\t" + c[0][i] + "\t" + d[0][i] + "\t" + xi[0][i]);
				}
				sw.WriteLine("");
				sw.WriteLine("E,MeV\tTaylor A1\tTaylor a1\tTaylor a2\tTaylor delta");
				for (int i = 0; i < u[0].Length; i++)
				{
					sw.WriteLine(Energy[i] + "\t" + tA1[0][i] + "\t" + tAlpha1[0][i] + "\t" + tAlpha2[0][i] + "\t" + delta[0][i]);
				}
			}
		}
		*/
	}
}
