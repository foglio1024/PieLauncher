using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace PieLauncher
{
    public class DateTimeToGradient : MarkupExtension, IValueConverter
    {
        static DateTimeToGradient? _instance;
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime dt) return null;

            //var minutes = dt.Hour * 60 + dt.Minute;
            //var factor = minutes / (24 * 60d);

            //var sunriseStart = 6 * 60;
            //var dayStart = 8 * 60;
            //var duskStart = 18 * 60;
            //var nightStart = 20 * 60;

            //if (minutes <= sunriseStart)
            //{
            //    var diff = minutes;
            //    var interval = sunriseStart - 0;

            //    var s1 = InterpolateAt(R.Resources.NightGradient1, R.Resources.SunriseGradient1, System.Convert.ToInt32((100 * diff / (double)interval)));
            //    var s2 = InterpolateAt(R.Resources.NightGradient2, R.Resources.SunriseGradient2, System.Convert.ToInt32((100 * diff / (double)interval)));

            //    return new LinearGradientBrush(s1, s2, 45);
            //}
            //if (minutes >= sunriseStart && minutes < dayStart)
            //{
            //    var diff = minutes - sunriseStart;
            //    var interval = dayStart - sunriseStart;

            //    var s1 = InterpolateAt(R.Resources.SunriseGradient1, R.Resources.AccentGradient1, System.Convert.ToInt32((100 * diff / (double)interval)));
            //    var s2 = InterpolateAt(R.Resources.SunriseGradient2, R.Resources.AccentGradient2, System.Convert.ToInt32((100 * diff / (double)interval)));

            //    return new LinearGradientBrush(s1, s2, 45);
            //}
            if (dt.Hour >= 6 && dt.Hour < 8) return R.Resources.SunriseGradient;
            if (dt.Hour >= 17 && dt.Hour < 21) return R.Resources.DuskGradient;
            if (dt.Hour >= 21 || dt.Hour < 6) return R.Resources.NightGradient;
            return R.Resources.DayGradient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="perc">0 - 100</param>
        /// <returns></returns>
        Color InterpolateAt(Color c1, Color c2, int perc)
        {
            int numberOfIntervals = 100; //or change to whatever you want.
            var interval_R = (c2.R - c1.R) / numberOfIntervals;
            var interval_G = (c2.G - c1.G) / numberOfIntervals;
            var interval_B = (c2.B - c1.B) / numberOfIntervals;

            var current_R = c1.R;
            var current_G = c1.G;
            var current_B = c1.B;
            var ret = c1;
            for (var i = 0; i <= numberOfIntervals; i++)
            {
                var color = Color.FromRgb(current_R, current_G, current_B);
                if (i == perc)
                {
                    ret = color;
                    break;
                }

                //increment.
                current_R += (byte)interval_R;
                current_G += (byte)interval_G;
                current_B += (byte)interval_B;
            }

            return ret;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_instance == null) _instance = new();
            return _instance;
        }
    }
}
