using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace PieLauncher.Avalonia.Controls;

public class DayOfTheWeekToBrush : MarkupExtension, IValueConverter
{
    public IBrush? Monday { get; set; }
    public IBrush? Tuesday { get; set; }
    public IBrush? Wednesday { get; set; }
    public IBrush? Thursday { get; set; }
    public IBrush? Friday { get; set; }
    public IBrush? Saturday { get; set; }
    public IBrush? Sunday { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dt) return null;

        return dt.DayOfWeek switch
        {
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            DayOfWeek.Sunday => Sunday,
            _ => null
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}