using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BSP.Source.Models.Converters
{
	[ValueConversion(typeof(double), typeof(string))]
	public class ConverterDouble : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value).ToString("#.##", culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double val = 0.0;
			if (double.TryParse((string)value, NumberStyles.Any, culture, out val))
			{
				return val;
			}
			return value;
		}
	}
}
