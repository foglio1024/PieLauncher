using System;
using Avalonia.Markup.Xaml;

namespace PieLauncher.Avalonia.Controls;

public class EnumValues : MarkupExtension
{
    private readonly Type _type;

    public EnumValues(Type enumType)
    {
        _type = enumType;
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValues(_type);
    }
}