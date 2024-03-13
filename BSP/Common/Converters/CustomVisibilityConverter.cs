﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BSP.Common.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class CustomVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool val) return null;
            return val ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
