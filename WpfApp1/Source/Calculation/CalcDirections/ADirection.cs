using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.Source.Calculation.CalcDirections
{
	public abstract class ADirection
	{
		/// <summary>
		/// Направление, вдоль которого рассчитывается мощность дозы
		/// </summary>
		public enum CalculationDirection
		{
			X, Y, Z
		}

		/// <summary>
		/// Enum for calculation direction
		/// </summary>
		public abstract CalculationDirection Direction { get; }

		public abstract string Name { get; }
	}
}
