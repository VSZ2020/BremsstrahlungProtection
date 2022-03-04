using BSP.Source.Calculation.CalcDirections;
using System;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;

namespace BSP
{
	public class Cylinder: ASourceForm
	{
		public Cylinder()
		{
			this.formType = FormType.Cylinder;
		}

		public override string Name => (string)Application.Current.Resources["SourceFormCylinder"];

		public override double GetVolume()
		{
			return Z * Math.PI / 4.0 * Y * Y;
		}

		public override Calculation.CalculationIntegral GetIntegralFunction(ADirection Direction)
		{
			if (Direction.Direction == ADirection.CalculationDirection.Z)
			{
				//Передача функции для осевой составляющей
				return CylinderAxial;
			}
			else
			{
				//Передача функции для радиальной составляющей
				return CylinderRadial;
			}
		}

		private static double CylinderRadial(ref InputData input, uint EnergyIndex)
		{
			//Задаем размеры источника
			double R = CalcParams.Source.Geometry.Y / 2.0;
			double _H = CalcParams.Source.Geometry.Z / 2.0;                     //берем для расчета половину высоты

			//Расстояние до точки измерения начиная от поверхности контейнера источника
			double b = CalcParams.CalculationDistance + R + input.Layers.D;

			//Объявляем перечень используемых переменных-итераций
			double _ro = 0.0; double _z = 0.0; double _Fi = 0.0;

			//Шаги интегрирования
			double dro = R / InputData.Ny;                                     //1 - Division by Y
			double dz = _H / InputData.Nz;                                     //2 - Division by Z
			double dFi = Math.PI / InputData.Nx;                               //0 - Division by X

			double currIntegral = 0.0;
			for (int i = 0; i < InputData.Ny; i++)                             //Цикл по радиусу
			{
				_ro = dro / 2 + dro * i;
				for (int j = 0; j < InputData.Nz; j++)                         //Цикл по высоте
				{
					_z = dz / 2 + dz * j;
					for (int l = 0; l < InputData.Nx; l++)                     //Цикл по углу
					{
						_Fi = dFi / 2 + dFi * l;

						double _cFull = _z * _z + _ro * _ro + b * b - 2 * _ro * b * Math.Cos(_Fi);
						double _c = _ro * _ro + b * b - 2 * _ro * b * Math.Cos(_Fi);

						//x - длина поглощения в источнике
						double _x = (_ro * _ro - _ro * b * Math.Cos(_Fi) +
										Math.Sqrt(R * R * _c - _ro * _ro * b * b * Math.Sin(_Fi) * Math.Sin(_Fi)))
										/ _c * Math.Sqrt(_cFull);

						double[] UD = AccessoryFunctions.GetUDForEnergy(ref input.UD[EnergyIndex], _x, Math.Sqrt(_cFull) / (b - _ro * Math.Cos(_Fi)));

						double sumUD = AccessoryFunctions.GetSumUD(UD);                        //Сумма всех UD по слоям для данной энергии

						double looseExp = Math.Exp(-sumUD);                                    //Полная экспонента ослабления

						//Расчет вклада поля рассеянного излучения
						double sumB = 1.0;                                                     //Равно 1.0 на случай, если не будет слагаемых
						if (input.IncludeScattering)
						{
							sumB = Buildup.GetGeteroBuildup(ref input, EnergyIndex, ref UD, ref input.token, Calculation.BuildupCalculationType);
						}

						//Текущее значение интеграла
						currIntegral += (double)CalcParams.multiplier * _ro / _cFull * dro * dz * dFi * looseExp * sumB;

						if (input.token.IsCancellationRequested)
						{
							return -1;
						}

					}
				}
			}
			return currIntegral * 4.0;
		}

		static public double CylinderAxial(ref InputData input, uint EnergyIndex)
		{
			//Переопределяем значения констант
			double cylR = CalcParams.Source.Geometry.Y / 2.0;
			double b = CalcParams.CalculationDistance + input.Layers.D;

			double O_1 = Math.Atan(cylR / (b + CalcParams.Source.Geometry.Z));                                  //0_1 - тета 1
			double O_2 = Math.Atan(cylR / b);                                                                   //0_2 - тета 2
			double dtheta_1 = O_1 / InputData.Nx;                                                              //шаг интегрирования для первого внешнего интеграла, d0_1
			double dtheta_2 = (O_2 - O_1) / InputData.Nx;                                                      //шаг интегрирования для второго внешнего интеграла, d0_2

			double externalIntergaral = 0.0;

			for (int i = 0; i < InputData.Nx; i++)                                                             //Цикл по углам "тета"
			{
				double thetaI_1 = dtheta_1 / 2.0 + dtheta_1 * i;                                                //Итерируемая тета для первого интеграла
				double thetaI_2 = O_1 + dtheta_2 / 2.0 + dtheta_2 * i;                                          //Итерируемая тета для второго интеграла

				//Для внутреннего интеграла
				double a_1 = b * AccessoryFunctions.sec(thetaI_1);                                              // a_1 = b*sec(0_1) - нижний предел для первого внутр. интеграла
				double a_2 = b * AccessoryFunctions.sec(thetaI_2);                                              // a_2 = b*sec(0_2) - нижний предел для второго внутр. интеграла

				double dr_1 = ((b + CalcParams.Source.Geometry.Z) / Math.Cos(thetaI_1) - a_1) / InputData.Ny;  //Шаг интегрирования для первого внутр. интеграла
				double dr_2 = (cylR * AccessoryFunctions.cosec(thetaI_2) - a_2) / InputData.Ny;                                //Шаг интегрирования для второго внутр. интеграла

				for (int j = 0; j < InputData.Ny; j++)                                                             //Цикл по радиусу (Y)
				{
					double _r_1 = a_1 + dr_1 / 2 + dr_1 * j;
					double _r_2 = a_2 + dr_2 / 2 + dr_2 * j;

					//Проверка внутреннего интеграла на пренебрежимо малое значение
					//if (Math.Exp(-_x2) * dr_2 < (1E-10)) dr_2 = 0;

					//double UDFactor_1 = 1.0 / Math.Cos(thetaI_1);													//Коэф. перехода к эффективному значению толщины
					//double UDFactor_2 = 1.0 / Math.Cos(thetaI_2);													//Коэф. перехода к эффективному значению толщины

					//Рассчитываем начальные проихведения u*d для всех слоев защиты, включая материал источника
					double[] UD_1 = AccessoryFunctions.GetUDForEnergy(ref input.UD[EnergyIndex], _r_1 - a_1, AccessoryFunctions.sec(thetaI_1));
					double[] UD_2 = AccessoryFunctions.GetUDForEnergy(ref input.UD[EnergyIndex], _r_2 - a_2, AccessoryFunctions.sec(thetaI_2));

					double sumUD_1 = AccessoryFunctions.GetSumUD(UD_1);
					double sumUD_2 = AccessoryFunctions.GetSumUD(UD_2);

					//Полные интегралы
					double cI_1 = dr_1 * Math.Sin(thetaI_1) * (double)CalcParams.multiplier * Math.Exp(-sumUD_1) * dtheta_1;
					double cI_2 = dr_2 * Math.Sin(thetaI_2) * (double)CalcParams.multiplier * Math.Exp(-sumUD_2) * dtheta_2;

					//Учет вклада поля рассеянного излучения
					double sumB_1 = 1.0;
					double sumB_2 = 1.0;                                                                            //Равно 1.0 на случай, если не будет слагаемых
					if (input.IncludeScattering)
					{
						sumB_1 = Buildup.GetGeteroBuildup(ref input, EnergyIndex, ref UD_1, ref input.token, Calculation.BuildupCalculationType);
						sumB_2 = Buildup.GetGeteroBuildup(ref input, EnergyIndex, ref UD_2, ref input.token, Calculation.BuildupCalculationType);
						if (double.IsNaN(sumB_1) || double.IsInfinity(sumB_1)) sumB_1 = 1;
						if (double.IsNaN(sumB_2) || double.IsInfinity(sumB_2)) sumB_2 = 1;
					}

					externalIntergaral += cI_1 * sumB_1 + cI_2 * sumB_2;                                            //Суммируем расчетные значения в один общий интеграл

					if (input.token.IsCancellationRequested) { return -1.0; }
				}
			}
			return externalIntergaral * 2.0 * Math.PI;
		}

	}
}
