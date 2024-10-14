using System.Windows;

namespace PieLauncher.Controls;

public class ContentControlExtensions
{
    public static string GetLabel(DependencyObject obj)
    {
        return (string)obj.GetValue(LabelProperty);
    }

    public static void SetLabel(DependencyObject obj, string value)
    {
        obj.SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.RegisterAttached("Label", typeof(string), typeof(ContentControlExtensions), new PropertyMetadata(""));
}
