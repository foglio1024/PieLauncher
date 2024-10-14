using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PieLauncher.Windows;

public class DateTimeToGradient : MarkupExtension, IValueConverter
{
    private static DateTimeToGradient? _instance;
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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
        return dt.Hour switch
        {
            >= 6 and < 8 => Resources.SunriseGradient,
            >= 17 and < 21 => Resources.DuskGradient,
            >= 21 or < 6 => Resources.NightGradient,
            _ => Resources.DayGradient
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _instance ??= new DateTimeToGradient();
    }
}