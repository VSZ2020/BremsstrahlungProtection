
namespace BSP
{
	/// <summary>
	/// Хранит интерполированные значения для заданного материала
	/// </summary>
	public class InterpolatedMaterial
	{
		public double[] um_Attenuation;
		public double[] um_Absorbtion;
		public InterpolatedTaylor Taylor;
		public InterpolatedKFactors KFactor;

		public InterpolatedMaterial(int EnergiesCount)
		{
			um_Absorbtion = new double[EnergiesCount];
			um_Attenuation = new double[EnergiesCount];
			Taylor = new InterpolatedTaylor(EnergiesCount);
			KFactor = new InterpolatedKFactors(EnergiesCount);
		}
	}
}
