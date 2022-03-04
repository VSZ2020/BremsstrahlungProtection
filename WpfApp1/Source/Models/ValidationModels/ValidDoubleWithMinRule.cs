using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace BSP
{
	public class ValidDoubleWithMinRule : ValidationRule
	{
		public double Min { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			double val = 0;
			try
			{
				val = double.Parse((string)value);
			}
			catch
			{
				return new ValidationResult(false, (string)Application.Current.Resources["msgError_IncorrectValueFormat"]);
			}

			if (val <= Min)
			{
				return new ValidationResult(false, string.Format((string)Application.Current.Resources["msgError_ValueMoreThan"], Min));
			}
			else
				return new ValidationResult(true, null);
		}
	}
}
