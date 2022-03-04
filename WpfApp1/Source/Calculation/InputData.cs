using BSP.Source.Calculation.CalcDirections;
using System;

namespace BSP
{
	/// <summary>
	/// Класс со входными данными для выполнения расчетов
	/// </summary>
	public class InputData
	{
		/// <summary>
		/// Набор интерполированных значений
		/// </summary>
		public InterpolatedData interpData;

		public ShieldLayers Layers;

		/// <summary>
		/// Флаг учета рассеяния
		/// </summary>
		public bool IncludeScattering;

		/// <summary>
		/// Предварительно рассчитанные u*d
		/// </summary>
		public double[][] UD;

		public IProgress<int> progressUpdater;
		public System.Threading.CancellationToken token;

		public ADirection CalcDirection;

		public static int Nx { get; set; } = 10;
		public static int Ny { get; set; } = 10;
		public static int Nz { get; set; } = 10;

		public InputData()
		{
			
		}

		/// <summary>
		/// Пересчитывает знаения u*d для каждого слоя защиты
		/// </summary>
		public void RecalcUD()
		{
			int LayersCount = interpData?.MaterialData?.Length ?? 0;
			int EnergiesCount = interpData?.Energy?.Length ?? 0;
			UD = new double[EnergiesCount][];
			for (int i = 0; i < EnergiesCount; i++)						//Цикл по энергиям
			{
				UD[i] = new double[LayersCount];
				for (int j = 0; j < LayersCount; j++)					//Цикл по слоям защиты
				{
					//Если j = 0 (слой материала источника), то ud = um(Source)[см2/г] * ro (Source)[г/см3] * 1 см
					if (j == 0)
						UD[i][0] = interpData.MaterialData[j].um_Attenuation[i] * CalcParams.Source.Density;
					else
						UD[i][j] = interpData.MaterialData[j].um_Attenuation[i] * Layers[j].dm;
				}
			}
		}
	}
}
