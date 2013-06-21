using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MegaDesktop.Ui
{
    public class GapToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool) value
                ? new Thickness(8, 0, 0, 0)
                : new Thickness(2, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}