using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace PieLauncher.Avalonia.Controls;

public class DateTimeToBrush : MarkupExtension, IValueConverter
{
    public IBrush? Morning { get; set; }
    public IBrush? Day { get; set; }
    public IBrush? Dusk { get; set; }
    public IBrush? Night { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTime dt) return null;
        
        return dt.Hour switch
        {
            >= 6 and < 8 => Morning,
            >= 17 and < 21 => Dusk,
            >= 21 or < 6 => Night,
            _ => Day
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