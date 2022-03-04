/*
 * Created by SharpDevelop.
 * User: Slava Izgagin
 * Date: 17-Feb-20
 * Time: 20:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Security;

namespace BSP
{

	public static class AccessoryFunctions
	{
		/// <summary>
		/// Возвращает массив u*d, с учетом преобразующих коэффициентов
		/// </summary>
		/// <param name="UD_InputArray">Массив u*d для заданной энергии по слоям защиты</param>
		/// <param name="source_d_factor">Модифицирующий фактор для толщины источника</param>
		/// <param name="shield_d_factor">Модифицирующий фактор для толщины защиты</param>
		/// <returns>Новый массив U*D</returns>
		public static double[] GetUDForEnergy(ref double[] UD_InputArray, double source_d_factor, double shield_d_factor)
		{
			var UD = new double[UD_InputArray.Length];
			if (!Calculation.IsPointSource)
			{
				//Для первого слоя будет свое произведение u*d
				UD[0] = UD_InputArray[0] * source_d_factor;
			}
			else
				UD[0] = 0;
			                                 

			for (int layerIndex = 1; layerIndex < UD.Length; layerIndex++)
			{
				UD[layerIndex] = UD_InputArray[layerIndex] * shield_d_factor;
			}

			return UD;
		}

		/// <summary>
		/// Возвращает сумму всех u*d массива
		/// </summary>
		/// <param name="UD">Массив произведений u*d</param>
		/// <returns></returns>
		public static double GetSumUD(double[] UD)
		{
			double sumUD = 0.0;
			for (int n = 0; n < UD.Length; n++)
			{
				sumUD += UD[n];
			}
			return sumUD;
		}

		public static double sec(double x)
		{
			return 1.0/Math.Cos(x);
		}

		public static double cosec(double x)
		{
			return 1.0 / Math.Sin(x);
		}
	}
}
