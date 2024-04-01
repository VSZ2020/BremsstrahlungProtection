using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BSP.Common.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InversedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool val) return null;
            return val ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
