using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HipMobileUI.Converters
{
    class TimeSpanDoubleConverter : IValueConverter
    {

        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan span = (TimeSpan)value;
            System.Diagnostics.Debug.WriteLine (span.TotalMilliseconds);
            return span.TotalMilliseconds;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMilliseconds ((double) value);
        }

    }
}
