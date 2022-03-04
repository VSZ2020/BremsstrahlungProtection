namespace BSP
{
	public class DoseFactorWithOrganName
	{
		/// <summary>
		/// Значения дозовых коэффициентов
		/// </summary>
		public FactorValue Factor;

		/// <summary>
		/// Название органа
		/// </summary>
		public string Name { get; set; }

		public DoseFactorWithOrganName(int EnergiesCount)
		{
			Factor = new FactorValue(EnergiesCount);
		}
	}
}
