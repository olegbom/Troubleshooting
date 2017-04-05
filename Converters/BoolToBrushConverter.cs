using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Troubleshooting.Converters
{
    class BoolToBrushConverter:IValueConverter
    {
        public Brush TrueBrush { get; set; }= Brushes.Red;
        public Brush FalseBrush { get; set; }= Brushes.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return TrueBrush;
            return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
