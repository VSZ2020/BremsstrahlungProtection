
namespace BSP
{ 
	public abstract class ASpline
	{
		/// <summary>
		/// Тип способа интерполяции
		/// </summary>
		public enum InterpolationType
		{
			Cubic,
			Linear
		}
		/// <summary>
		/// Хранит метод интерполяции
		/// </summary>
		public InterpolationType SplineType;

		public abstract double GetValue(double x, float EnergyLimit);

		/// <summary>
		/// Возвращает массив интерполированных значений для набора X
		/// </summary>
		/// <param name="x">Массив точек</param>
		/// <returns></returns>
		public abstract double[] GetArray(double[] x, ref Material material);
	}
}
