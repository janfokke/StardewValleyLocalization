using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StardewValleyLocalization.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class WarningLevelToIconConverter : IValueConverter
    {
        private static readonly BitmapImage InfoImage = new BitmapImage(new Uri(@"/Images/info.png", UriKind.Relative));

        private static readonly BitmapImage WarningImage =
            new BitmapImage(new Uri(@"/Images/warning.png", UriKind.Relative));

        private static readonly BitmapImage ErrorImage =
            new BitmapImage(new Uri(@"/Images/error.png", UriKind.Relative));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var warningLevel = (WarningLevel) value;
            switch (warningLevel)
            {
                case WarningLevel.Info:
                    return InfoImage;
                case WarningLevel.Warning:
                    return WarningImage;
                default:
                    return ErrorImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}