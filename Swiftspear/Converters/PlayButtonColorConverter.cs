using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Swiftspear.Converters
{
    class PlayButtonColorConverter : IValueConverter
    {
        private readonly static Style PlayingStyle;
        private readonly static Style StoppedStyle;

        static PlayButtonColorConverter()
        {
            PlayingStyle = new Style(typeof(Label));
            PlayingStyle.Setters.Add(new Setter(Label.BackgroundProperty, Brushes.Transparent));
            PlayingStyle.Setters.Add(new Setter(Label.ForegroundProperty, Brushes.LimeGreen));

            StoppedStyle = new Style(typeof(Label));
            StoppedStyle.Setters.Add(new Setter(Label.BackgroundProperty, Brushes.Transparent));
            StoppedStyle.Setters.Add(new Setter(Label.ForegroundProperty, Brushes.Gray));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(bool) || targetType != typeof(Style))
            {
                throw new ArgumentException();
            }

            // is playing
            if ((bool)value)
            {
                return PlayingStyle;
            }
            else
            {
                return StoppedStyle;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
