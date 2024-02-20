/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 07-Mar-20
 * Время: 14:00
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Windows;

namespace BSP
{
	/// <summary>
	/// Класс, описывающий характеристики тормозного излучения
	/// </summary>
	public static class Breamsstrahlung
	{
		private static double[] _BsFactor;
		private static double[,] _energyFactors;
		
		public static int Length
		{
			get {
				 return _BsFactor?.Length ?? 0;
			}
		}
		
		public static double[] BsFactor
		{
			get 
			{
				if (_BsFactor == null)
					SetDefaultBSFactors(); 
				return _BsFactor;
			}
			set
			{
				if (value == null)
					SetDefaultBSFactors();
				else
					_BsFactor = value;
			}
		}
		
		public static double[,] EnergyFactor
		{
			get
			{
				if (_energyFactors == null)
					_energyFactors = SetDefaultEnergyFactors();
				return _energyFactors;
			}
			set
			{
				if (value == null)
					_energyFactors = SetDefaultEnergyFactors();
				else
					_energyFactors = value;
			}
		}

		/// <summary>
		/// Задание дефолтных значений долей от полного выхода ТИ
		/// </summary>
		public static void SetDefaultBSFactors()
		{
			_BsFactor = new double[] { 0.435, 0.258, 0.152, 0.083, 0.043, 0.02, 7.0E-3, 2.0E-3, 3.0E-4,0 };
		}
		
		public static double[,] SetDefaultEnergyFactors()
		{
			double[,] factors = new double[_BsFactor.Length,2];
			for (int i = 0; i < _BsFactor.Length; i++)
			{
				factors[i,0] = 0.1*i;
				factors[i,1] = 0.1*(i+1);
			}
			return factors;
		}

		/// <summary>
		/// Находит из перечня нуклидов изотоп с максимальным произведением I*E
		/// </summary>
		/// <param name="NuclidesList"></param>
		/// <param name="PrimeEnergyIndex"></param>
		/// <param name="PrimeNuclideName"></param>
		public static int SearchPrimaryNuclide(Nuclides NuclidesList)
		{
			int PrimeEnergyIndex = 0;
			double maxEnergy = 0.0;                                                 //Переменная для поиска максимального значения
			int count = NuclidesList.Length;

			for (int i = 0; i < count; i++)
			{
				int enCount = NuclidesList.Collection[i].listEnergyYield.Count;
				for (int j = 0; j < enCount; j++)
				{
					var ncl = NuclidesList.Collection[i];
					if (ncl.listMaxEnergy[j] * ncl.listEnergyYield[j] > maxEnergy)
					{
						maxEnergy = ncl.listMaxEnergy[j] * ncl.listEnergyYield[j];
						PrimeEnergyIndex = j;
						CalcParams.generalNuclideKey = ncl.Name;
					}
				}
			}
			return PrimeEnergyIndex;
		}

		/// <summary>
		/// Вычисляет поток энергии Ibeta maximum
		/// </summary>
		/// <param name="PrimaryNuclide">Нуклид, по которому счиатеся поток энергии</param>
		/// <returns></returns>
		private static double IbetaMax(Nuclide PrimaryNuclide, float Z_eff)
		{
			double sumI = 0.0;
			for (int i = 0; i < PrimaryNuclide.listMeanEnergy.Count; i++)
			{
				sumI += PrimaryNuclide.listMeanEnergy[i] * PrimaryNuclide.listMeanEnergy[i] * PrimaryNuclide.listEnergyYield[i];
			}

			return (8.5E-4) * ((double)Z_eff + 3.0) * sumI;

		}

		//Расчет общего потока энергии от набора нуклидов
		public static double[] GetEnergyFlows(Nuclides SelectedNuclides, float Z_eff)
		{
			if (SelectedNuclides == null | SelectedNuclides.Length == 0) throw new Exception((string)Application.Current.Resources["msgError_NullOrZeroNuclides"]);

			int nuclidesCount = SelectedNuclides.Length;																		//Количество нуклидов в списке

			//Поиск нуклида с максимальным I*E
			int maxIndex = SearchPrimaryNuclide(SelectedNuclides);																//Получаем индекс главной линии для доминирующего нуклида

			//Создание массива для потоков энергии
			double[] Ib = new double[Length];
			double IbMax = IbetaMax(SelectedNuclides[CalcParams.generalNuclideKey], Z_eff);										//Вычисление Ibeta-max для доминирующего нуклида

			//Вычисление потока энергии для базового нуклида и для каждой эн. группы
			for (int i = 0; i < Length; i++)
			{
				Ib[i] = IbMax * BsFactor[i];																					//Потоки энергии от основного нуклида
			}

			return Ib;
		}

		
	}
}
