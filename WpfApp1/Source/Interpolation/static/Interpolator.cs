

using System;

namespace BSP
{
	public static class Interpolator
	{
		/// <summary>
		/// Проводит интерполяцию в взависимости от вида сплайна 
		/// </summary>
		/// <param name="X">Табличные значения энергий</param>
		/// <param name="Y">Табличные значения параметра</param>
		/// <param name="InterpolationType">Тип интерполяционного полинома</param>
		/// <param name="NewX">Значения энергий, для которых получают новые значения</param>
		/// <returns></returns>
		private static double[] Interpolate(double[] X, double[] Y, ASpline.InterpolationType InterpolationType, double[] NewX, ref Material Layer)
		{
			return (InterpolationType == ASpline.InterpolationType.Cubic) ? (new CSpline(X,Y)).GetArray(NewX, ref Layer) : (new LSpline(X, Y)).GetArray(NewX, ref Layer);
		}

		/// <summary>
		/// Возвращает набор интерполированных значений
		/// </summary>
		/// <param name="Energy"></param>
		/// <returns></returns>
		public static InterpolatedData GetInterpolatedData(ref InputData input, double[] Energy, FactorValue selDoseFactor)
		{
			int ShieldLayersCount = input.Layers.Count;
			InterpolatedData data = new InterpolatedData(Breamsstrahlung.Length, ShieldLayersCount);
			data.Energy = Energy;
			
			for (int i = 0; i < ShieldLayersCount; i++)
			{
				var material = input.Layers[i].Material;
				var Factors = material.Factors;
				//um (absorbtion)
				data.MaterialData[i].um_Absorbtion = Interpolate(Factors.Um_absorbtion.Energy, Factors.Um_absorbtion.Value, ASpline.InterpolationType.Linear, Energy, ref material);
				//um (attenuation)
				data.MaterialData[i].um_Attenuation = Interpolate(Factors.Um_attennuation.Energy, Factors.Um_attennuation.Value, ASpline.InterpolationType.Linear, Energy, ref material);

				//KFactor a
				data.MaterialData[i].KFactor.a = Interpolate(Factors.KFactors.Energy, Factors.KFactors.a, ASpline.InterpolationType.Linear, Energy, ref material);
				//KFactor b
				data.MaterialData[i].KFactor.b = Interpolate(Factors.KFactors.Energy, Factors.KFactors.b, ASpline.InterpolationType.Linear, Energy, ref material);
				//KFactor c
				data.MaterialData[i].KFactor.c = Interpolate(Factors.KFactors.Energy, Factors.KFactors.c, ASpline.InterpolationType.Linear, Energy, ref material);
				//KFactor d
				data.MaterialData[i].KFactor.d = Interpolate(Factors.KFactors.Energy, Factors.KFactors.d, ASpline.InterpolationType.Linear, Energy, ref material);
				//KFactor xk
				data.MaterialData[i].KFactor.xk = Interpolate(Factors.KFactors.Energy, Factors.KFactors.xk, ASpline.InterpolationType.Linear, Energy, ref material);

				//Taylor A1
				data.MaterialData[i].Taylor.A1 = Interpolate(Factors.Taylor.Energy, Factors.Taylor.A1, ASpline.InterpolationType.Cubic, Energy, ref material);
				//Taylor a1
				data.MaterialData[i].Taylor.a1 = Interpolate(Factors.Taylor.Energy, Factors.Taylor.a1, ASpline.InterpolationType.Cubic, Energy, ref material);
				//Taylor a2
				data.MaterialData[i].Taylor.a2 = Interpolate(Factors.Taylor.Energy, Factors.Taylor.a2, ASpline.InterpolationType.Cubic, Energy, ref material);
				//Taylor delta
				data.MaterialData[i].Taylor.Delta = Interpolate(Factors.Taylor.Energy, Factors.Taylor.Delta, ASpline.InterpolationType.Cubic, Energy, ref material);
			}
			//Dose factor
			var mat = new Material("", 1) { EnergyLimit = 1000 };
			data.DoseFactor = Interpolate(selDoseFactor.Energy, selDoseFactor.Value, ASpline.InterpolationType.Cubic, Energy, ref mat) ;

			try
			{
				//Air factor interpolation
				var materialAir = CalcParams.TableMaterials[CalcParams.environmentKey];
				var factor = materialAir.Factors.Um_absorbtion;
				data.um_absorbtion_air = Interpolate(factor.Energy, factor.Value, ASpline.InterpolationType.Linear, Energy, ref materialAir);
			}
			catch
			{
				throw;
			}
			
			return data;
		}
	}
}
