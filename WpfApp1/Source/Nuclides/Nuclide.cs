/*
 * Created by SharpDevelop.
 * User: Slava Izgagin
 * Date: 17-Feb-20
 * Time: 00:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections.Generic;

namespace BSP
{
	public class Nuclide
	{
		private string nuclideName;             //Имя нуклида
		private double _Activity;

		/// <summary>
		/// Активность нуклида в Беккрелях
		/// </summary>
		public double Activity                  //Активность отдельного нуклида в смеси
		{
			get
			{
				return _Activity;
			}
			set
			{
				_Activity = (value >= 0.0) ? value : throw new System.Exception("Значение активности не может быть отрицательным");
			}
		}

		/// <summary>
		/// Период полураспада в сутках
		/// </summary>
		public double HalfTime;                 //Период полураспада

		/// <summary>
		///Набор средних и граничных значений энергии для каждой энергетической группы 
		/// </summary>
		public BsSpectrum BSEnergySpectrum;

		//public Nuclides ChuildNuclides;
		
		public List<double> listMaxEnergy;		//Набор максимальных энергий нуклида, [МэВ]
		public List<double> listMeanEnergy;		//Набор средних энергий нуклида, [МэВ/част.]
		public List<double> listEnergyYield;	//Набор интенсивности линии излучения, [доли]
		
		/// <summary>
		/// Возвращает или устанавливает название радионуклида
		/// </summary>
		public string Name 
		{
			get {return nuclideName;}
			set {nuclideName = (string.IsNullOrEmpty(value)) ? "Безымянный нуклид": value;}
		}
		
		public Nuclide(string NuclideName, List<double> LMaxEnergies, List<double> LMeanEnergies, List<double> LYields)
		{
			this.Name = NuclideName;
			
			listMaxEnergy 	= LMaxEnergies ?? new List<double>();
			listMeanEnergy 	= LMeanEnergies ?? new List<double>();
			listEnergyYield = LYields ?? new List<double>();

			RecalculateMeanEnergies();
		}
		
		/// <summary>
		/// Пересчитывает групповые энергии тормозного излучения для данного нуклида
		/// </summary>
		public void RecalculateMeanEnergies()
		{
			double maxEnergy = 0.0;

			//Поиск максимальной линии энергии с максимальным вкладом
			for (int i = 0; i < listMaxEnergy.Count; i++)
			{
				if (listMaxEnergy[i] * listEnergyYield[i] > maxEnergy) maxEnergy = listMaxEnergy[i];
			}

			int groupsCount = Breamsstrahlung.Length;
			BSEnergySpectrum = new BsSpectrum(groupsCount);

			//Вычисление всех групповых средних и граничных энергий
			for (int i = 0; i < groupsCount; i++)
			{
				BSEnergySpectrum.DOWN_GroupEnergy[i] = Breamsstrahlung.EnergyFactor[i, 0] * maxEnergy;
				BSEnergySpectrum.UP_GroupEnergy[i] = Breamsstrahlung.EnergyFactor[i, 1] * maxEnergy;
				BSEnergySpectrum.MeanEnergies[i] = (BSEnergySpectrum.UP_GroupEnergy[i] + BSEnergySpectrum.DOWN_GroupEnergy[i]) / 2.0;
			}
		}
	}
}
