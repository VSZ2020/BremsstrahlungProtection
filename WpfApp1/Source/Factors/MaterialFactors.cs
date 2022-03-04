

namespace BSP
{
	public class MaterialFactors
	{
		/// <summary>
		/// Массовый коэффициент ослабления
		/// </summary>
		public FactorValue Um_attennuation;

		/// <summary>
		/// Массовый коэффициент поглощения
		/// </summary>
		public FactorValue Um_absorbtion;

		/// <summary>
		/// Коэффициенты формулы Тейлора
		/// </summary>
		public TaylorFactors Taylor;

		/// <summary>
		/// Коэффициенты японской формулы
		/// </summary>
		public KFactors KFactors;

		public MaterialFactors()
		{
			/*
			Um_attennuation = new FactorValue(EnergiesCount);
			Um_absorbtion = new FactorValue(EnergiesCount);
			Taylor = new TaylorFactors(EnergiesCount);
			KFactors = new KFactors(EnergiesCount);
			*/
		}
	}
}
