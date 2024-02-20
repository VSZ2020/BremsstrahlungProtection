using System;
using System.Threading;

namespace BSP
{
	public static class Buildup
	{
		public const double tanh2_1 = 1.96402758007581688395;
		public const double tanh2 = -0.96402758007581688395;
		/// <summary>
		/// Расчет фактора накопления по формуле из Radiological Toolbox
		/// </summary>
		/// <param name="ud"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <param name="xi"></param>
		/// <returns></returns>
		private static double JapanBuildup(ref InterpolatedKFactors f, uint EnergyIndex, double ud)
		{
			double K = f.c[EnergyIndex] * Math.Pow(ud, f.a[EnergyIndex]) + f.d[EnergyIndex] * (Math.Tanh(ud / f.xk[EnergyIndex] - 2.0) - tanh2) / tanh2_1;
			//if (double.IsNaN(K))
			//	System.Diagnostics.Debug.WriteLine($"NaN value for K: c={f.c[EnergyIndex]}, ud={ud}, d={f.d[EnergyIndex]}, a={f.a[EnergyIndex]}, mathPow={Math.Pow(ud, f.a[EnergyIndex])}, tanh={Math.Tanh(ud / f.xk[EnergyIndex] - 2.0)}, tanh2Big={(Math.Tanh(ud / f.xk[EnergyIndex] - 2.0) - tanh2)}", "JapanFormula");
			return (K == 1) ? 1.0 + (f.b[EnergyIndex] - 1.0) * ud : 1.0 + (f.b[EnergyIndex] - 1.0) * (Math.Pow(K, ud) - 1.0) / (K - 1.0);
		}

		private static double TaylorBuildup(ref InterpolatedTaylor f, uint EnergyIndex, double ud)
		{
			return f.A1[EnergyIndex] * Math.Exp(-f.a1[EnergyIndex] * ud) + (1 - f.A1[EnergyIndex]) * Math.Exp(-f.a2[EnergyIndex] * ud);
		}

		/// <summary>
		/// Возвращает вычисленный фактор накопления выбранным методом
		/// </summary>
		/// <param name="Data">Входные параметры</param>
		/// <param name="EnergyIndex"></param>
		/// <param name="BuildupType"></param>
		/// <returns></returns>
		public static double GetGeteroBuildup(InputData Data, uint EnergyIndex, double[] ud, ref CancellationToken token, Calculation.BuildupCalcType BuildupType)
		{
			//Проверяем флаг, что источник точечный и слоев защите нет и флаг того, что энергия излучения слишком мала и всё поглотится
			if ((Calculation.IsPointSource && ud.Length == 1) || Data.interpData.Energy[EnergyIndex] <= Calculation.BUILDUP_DOWN_ENERGY_LIMIT)
			{
					return 1.0;
			}
				
			//Проверяем выбранный метод расчета
			if (BuildupType == Calculation.BuildupCalcType.Taylor)
			{
				return GetGeteroBuildup_Taylor(ref Data, ref ud, EnergyIndex, ref token);
			}
			return GetGeteroBuildup_Japan(ref Data, ref ud, EnergyIndex, ref token);
		}

		/// <summary>
		/// Возвращает фактор накопления для гетерогенной защиты, рассчитанный по формулам Тейлора
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="ud"></param>
		/// <param name="EnergyIndex"></param>
		/// <returns></returns>
		private static double GetGeteroBuildup_Taylor(ref InputData Data, ref double[] ud, uint EnergyIndex, ref CancellationToken token)
		{
			//[Первое слагаемое формулы Бродера]
			int layersCount = Data.Layers.Count;

			double sumB = TaylorBuildup(ref Data.interpData.MaterialData[layersCount - 1].Taylor, EnergyIndex, AccessoryFunctions.GetSumUD(ud)) * Data.interpData.MaterialData[layersCount - 1].Taylor.Delta[EnergyIndex];

			//Последующие слагаемые. 
			//[Внешняя сумма]
			for (int layerIndex = 0; layerIndex < layersCount - 1; layerIndex++)
			{
				double sumUD = 0.0;                                                     //Хранит суммарное u*d для данного Bn
				for (int n = 0; n <= layerIndex; n++)
				{
					sumUD += ud[n];
				}

				sumB += TaylorBuildup(ref Data.interpData.MaterialData[layerIndex].Taylor,  EnergyIndex, sumUD) * Data.interpData.MaterialData[layerIndex].Taylor.Delta[EnergyIndex]
						-
						TaylorBuildup(ref Data.interpData.MaterialData[layerIndex + 1].Taylor, EnergyIndex, sumUD) * Data.interpData.MaterialData[layerIndex + 1].Taylor.Delta[EnergyIndex];

				//Если отмена, то остановка вычислений
				if (token.IsCancellationRequested) { return -1.0; }
			}
			return sumB;
		}

		private static double GetGeteroBuildup_Japan(ref InputData Data, ref double[] ud, uint EnergyIndex, ref CancellationToken token)
		{
			//[Первое слагаемое формулы Бродера]
			int layersCount = Data.Layers.Count;

			double sumB = JapanBuildup(ref Data.interpData.MaterialData[layersCount - 1].KFactor, EnergyIndex,	AccessoryFunctions.GetSumUD(ud)) * Data.interpData.MaterialData[layersCount - 1].Taylor.Delta[EnergyIndex];
			
			//Последующие слагаемые формулы Бродера 
			//[Внешняя сумма]
			for (int layerIndex = 0; layerIndex < layersCount - 1; layerIndex++)
			{
				double sumUD = 0.0;                                                     //Хранит суммарное u*d для данного B[n] и B[n+1]
				for (int n = 0; n <= layerIndex; n++)
				{
					sumUD += ud[n];
				}

				sumB += JapanBuildup(ref Data.interpData.MaterialData[layerIndex].KFactor, EnergyIndex, sumUD) * Data.interpData.MaterialData[layerIndex].Taylor.Delta[EnergyIndex] 
						-
						JapanBuildup(ref Data.interpData.MaterialData[layerIndex + 1].KFactor, EnergyIndex, sumUD) * Data.interpData.MaterialData[layerIndex + 1].Taylor.Delta[EnergyIndex];

				//Если отмена, то остановка вычислений
				if (token.IsCancellationRequested) { return -1.0; }
			}
			return sumB;
		}
	}
}
