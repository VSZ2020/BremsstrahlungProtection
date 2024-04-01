using System.Globalization;
using System.Windows.Data;

namespace BSP.Common.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InversedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool val) return null;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool val) return null;
            return !val;
        }
    }
}
