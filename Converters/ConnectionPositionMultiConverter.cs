using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Troubleshooting
{
    public class ConnectionPositionMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double canvasLeft = (double)values[0];
            double canvasTop = (double)values[1];
            double actualWidth = (double)values[2];
            double actualHeight = (double)values[3];
            return new Point(canvasLeft + actualHeight, canvasTop + actualHeight/2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
