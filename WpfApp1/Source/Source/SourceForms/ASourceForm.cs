using BSP.Source.Calculation.CalcDirections;
using System;
using System.Collections.Generic;

namespace BSP
{
	public abstract class ASourceForm
	{
		private double _x = 10.0;
		private double _y = 10.0;
		private double _z = 10.1;

		public enum FormType
		{
			Cylinder,
			Parallelepiped
		}

		public FormType formType;

		public double X
		{
			get { return _x; }
			set
			{
				_x = (value > 0) ? value : throw new ArgumentOutOfRangeException("Длина (X) источника должна быть > 0");
			}
		}
		public double Y
		{
			get { return _y; }
			set
			{
				_y = (value > 0F) ? value : throw new ArgumentOutOfRangeException("Ширина (Y) источника должна быть > 0");
			}
		}
		public double Z
		{
			get { return _z; }
			set
			{
				_z = (value > 0F) ? value : throw new ArgumentOutOfRangeException("Высота (Z) источника должна быть > 0");
			}
		}

		/// <summary>
		/// Возвращает объем источника в см^3
		/// </summary>
		public abstract double GetVolume();

		/// <summary>
		/// Geomerty form name
		/// </summary>
		public abstract string Name { get; }

		public abstract Calculation.CalculationIntegral GetIntegralFunction(ADirection Direction);
	}
}
