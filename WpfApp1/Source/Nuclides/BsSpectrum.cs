

namespace BSP
{
	public class BsSpectrum
	{
		/// <summary>
		/// Средние энергии групп
		/// </summary>
		public double[] MeanEnergies;

		/// <summary>
		/// Верхняя энергетическая граница группы
		/// </summary>
		public double[] UP_GroupEnergy;

		/// <summary>
		/// Нижняя энергетическая граница группы
		/// </summary>
		public double[] DOWN_GroupEnergy;

		public int Length
		{
			get { return MeanEnergies?.Length ?? 0; }
		}

		public BsSpectrum(int GroupCount)
		{
			MeanEnergies = new double[GroupCount];
			UP_GroupEnergy = new double[GroupCount];
			DOWN_GroupEnergy = new double[GroupCount];
		}
	}
}
