using BSP.Source.Calculation.CalcDirections;
using System;
using System.Threading.Tasks;

namespace BSP
{
	public class Calculation
	{
		/*
		 * Вести расчет только воздушной кермы. Если пользователь потребует, то домножать все результаты на модифицирующий коэффициент
		 * 
		 */
		
		public enum BuildupCalcType
		{
			Taylor = 0,
			Toolbox
		}

		/// <summary>
		/// Флаг метода расчета ФН
		/// </summary>
		public static BuildupCalcType BuildupCalculationType { get; set; } = BuildupCalcType.Toolbox;				//Константа, задающая способ расчета ФН
                     
		/// <summary>
		/// Флаг того, что рассчитываемый источник точечный
		/// </summary>
		public static bool IsPointSource { get; set; } = false;
		/// <summary>
		/// Энергетический порог, ниже которого все излучение поглощается полностью, а значит фактор накопления равен 1.
		/// </summary>
		public const double BUILDUP_DOWN_ENERGY_LIMIT = 0.015;

		public delegate double CalculationIntegral(ref InputData Data, uint EnergyIndex);

		public Calculation()
		{
				
		}

		public async Task<OutputValue> StartAsync(InputData input)
		{
			//Создаем экземпляр делегата и Назначаем обрабатывающую функцию в зависимости от формы и направления
			CalculationIntegral cFunction = CalcParams.Source.Geometry.GetIntegralFunction(input.CalcDirection);

			//Создаем выходной массив
			int EnergiesCount = Breamsstrahlung.Length;
			OutputValue doseRate = new OutputValue();
			doseRate.DoseRatePart = new double[EnergiesCount];

			for (uint EnergyIndex = 0; EnergyIndex < EnergiesCount; EnergyIndex++)                  //Цикл по энергиям
			{
				//Вычисляем интеграл в другом потоке
				double Integral = await Task.Run(() => cFunction(ref input, EnergyIndex));
				
				//Вычисляем парциальную мощность воздушной кермы: K = [МэВ/(с * г) → Гр/ч] / (4 * pi) * k * Am * Ib * um(air) * Intergral
				doseRate.DoseRatePart[EnergyIndex] =
					(1.6E-10) / (4.0 * Math.PI) * 3600 * input.interpData.DoseFactor[EnergyIndex] * CalcParams.Source.SpecificActivity * CalcParams.Source.Ib[EnergyIndex] * input.interpData.um_absorbtion_air[EnergyIndex] * Integral;

				//Если значение NaN, то ...
				if (double.IsNaN(doseRate.DoseRatePart[EnergyIndex]))
				{
					doseRate.DoseRatePart[EnergyIndex] = 0.0;
					//Записываем в лог
				}
				//Если значение Inf, то ...
				if (double.IsInfinity(doseRate.DoseRatePart[EnergyIndex]))
				{
					doseRate.DoseRatePart[EnergyIndex] = 0.0;
					//Записываем в лог
				}

				if (input.progressUpdater != null) input.progressUpdater.Report((int)EnergyIndex + 1);
			}

			return doseRate;
		}
	}
}
