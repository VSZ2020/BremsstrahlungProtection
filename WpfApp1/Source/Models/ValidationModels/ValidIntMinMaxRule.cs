using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace BSP
{
	public class ValidIntMinMaxRule : ValidationRule
	{
		private int minValue;
		private int maxValue;

		public int Min
		{
			get { return minValue; }
			set { minValue = value; }
		}

		public int Max { get { return maxValue; } set { maxValue = value; } }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			int val = 0;
			try
			{
				val = int.Parse((string)value);
			}
			catch
			{
				return new ValidationResult(false, (string)Application.Current.Resources["msgError_IncorrectValueFormat"]);
			}

			if ((val < Min) || (val > Max))
			{
				return new ValidationResult(false, string.Format((string)Application.Current.Resources["msgError_ValueBetween"], Min, Max));
			}
			else
				return new ValidationResult(true, null);
		}
	}
}
