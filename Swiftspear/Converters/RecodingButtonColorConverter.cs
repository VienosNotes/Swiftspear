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
    class RecodingButtonColorConverter : IValueConverter
    {
        private readonly static Style RecordingStyle;
        private readonly static Style StoppedStyle;

        static RecodingButtonColorConverter()
        {
            RecordingStyle = new Style(typeof(Label));
            RecordingStyle.Setters.Add(new Setter(Label.BackgroundProperty, Brushes.LimeGreen));
            RecordingStyle.Setters.Add(new Setter(Label.ForegroundProperty, Brushes.Black));

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

            // IsRecording
            if ((bool)value)
            {
                return RecordingStyle;
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
