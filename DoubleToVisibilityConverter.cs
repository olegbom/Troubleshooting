using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Troubleshooting
{
    public class DoubleToVisibilityConverter : IValueConverter
    {
        public double Threshold;
        public bool Less;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = Less 
                ? Threshold < (double) value 
                : Threshold >= (double) value;
            return result 
                ? Visibility.Visible 
                : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}