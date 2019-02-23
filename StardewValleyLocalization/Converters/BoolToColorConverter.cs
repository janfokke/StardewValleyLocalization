using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StardewValleyLocalization.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush Red = new SolidColorBrush(Colors.Gray);
        private static readonly SolidColorBrush Black = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? Red : Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}