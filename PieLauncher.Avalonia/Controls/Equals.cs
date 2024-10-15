using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

using System;
using System.Globalization;

namespace PieLauncher.Avalonia.Controls;

public class Equals : MarkupExtension, IValueConverter
{
    public double Value { get; set; }
    public bool Invert { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            var v = System.Convert.ToDouble(value);

            return Invert
                ? v != Value
                : v == Value;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}