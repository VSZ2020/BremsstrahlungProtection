
using BSP.Source.Calculation.CalcDirections;
using System.Threading.Tasks;
using System.Windows;

namespace BSP
{
	public class Parallelepiped : ASourceForm
	{
		private byte DirectionFlag = 1;

		public Parallelepiped()
		{
			this.formType = FormType.Parallelepiped;
		}

		public override double GetVolume()
		{
			return X * Y * Z;
		}

		public override string Name => (string)Application.Current.Resources["SourceFormParallelepiped"];

		public override Calculation.CalculationIntegral GetIntegralFunction(ADirection Direction)
		{
			if (Direction.Direction == ADirection.CalculationDirection.X)
			{
				//Передача функции для радиальной составляющей
				DirectionFlag = 1;
			}
			if (Direction.Direction == ADirection.CalculationDirection.Y)
			{
				//Передача функции для радиальной составляющей
				DirectionFlag = 2;
			}
			if (Direction.Direction == ADirection.CalculationDirection.Z)
			{
				//Передача для осевой составляющей
				DirectionFlag = 3;
			}
			return CubeRadial;
		}
		/// <summary>
		/// Вычисление радиальной составляющей излучения от прямоугольного параллелепипеда
		/// </summary>
		/// <param name="input"></param>
		/// <param name="EnergyIndex"></param>
		/// <returns></returns>
		private double CubeRadial(InputData input, uint EnergyIndex)
		{
			//Расстояние до точки детектирования
			double _b = CalcParams.CalculationDistance + input.Layers.D;

			//Переменные для расчетов
			double dx = CalcParams.Source.Geometry.X / InputData.Nx;
			double dy = CalcParams.Source.Geometry.Y / InputData.Ny;
			double dz = CalcParams.Source.Geometry.Z / InputData.Nz;

			//Начальные координаты точки регистрации
			double x0 = CalcParams.Source.Geometry.X / 2.0;
			double y0 = CalcParams.Source.Geometry.Y / 2.0;
			double z0 = CalcParams.Source.Geometry.Z / 2.0;

			if (DirectionFlag == 1) { x0 = _b + CalcParams.Source.Geometry.X; }                             //1 - излучение сдоль оси X,
			if (DirectionFlag == 2) { y0 = _b + CalcParams.Source.Geometry.Y; }                             //2 - вдоль оси Y,
			if (DirectionFlag == 3) { z0 = _b + CalcParams.Source.Geometry.Z; }                             //3 - вдоль оси Z

			double sumIntegral = 0.0;

			for(int i = 0; i < InputData.Nx && !input.token.IsCancellationRequested ;i++)
			{
				  double _x = dx / 2 + dx * i;
				  for (int j = 0; j < InputData.Ny && !input.token.IsCancellationRequested; j++)
				  {
					  double _y = dy / 2 + dy * j;
					  for (int _l = 0; _l < InputData.Nz && !input.token.IsCancellationRequested; _l++)
					  {
						  double _z = dz / 2 + dz * _l;
						  double _c = 1.0;                                                                                    //Переменнная, определяемая направлением точки регистрации
						if (DirectionFlag == 1) { _c = CalcParams.Source.Geometry.X - _x; }
						  if (DirectionFlag == 2) { _c = CalcParams.Source.Geometry.Y - _y; }                                 //2 - вдоль оси Y,
						if (DirectionFlag == 3) { _c = CalcParams.Source.Geometry.Z - _z; }                                 //3 - вдоль оси Z

						double R2 = (_x - x0) * (_x - x0) + (_y - y0) * (_y - y0) + (_z - z0) * (_z - z0);                  //Квадрат расстояния от объема до точки регистрации R
						double xe = _c / (_b + _c) * System.Math.Sqrt(R2);                                                  //Длина самопоглощения в источнике

						//Рассчитываем начальные проихведения u*d для всех слоев защиты, включая материал источника
						double UDFactor = R2 / (_b + _c);                                                                   //Коэф. перехода к эффективному значению u*d
						double[] UD = AccessoryFunctions.GetUDForEnergy(input.UD[EnergyIndex], xe, UDFactor);

						  double sumUD = AccessoryFunctions.GetSumUD(UD);
						  double looseExp = System.Math.Exp(-sumUD);

						//Расчет вклада поля рассеянного излучения
						double sumB = 1.0;                                                                                  //Равно 1.0 на случай, если не будет слагаемых
						if (input.IncludeScattering)
						  {
							  sumB = Buildup.GetGeteroBuildup(input, EnergyIndex, UD, ref input.token, Calculation.BuildupCalculationType);
						  }

						  sumIntegral += (double)CalcParams.multiplier * looseExp / R2 * dx * dy * dz * sumB;
					  }
				  }
			}
			if (input.token.IsCancellationRequested) { 
				return -1.0;
			}
			return sumIntegral;
		}
	}
}
