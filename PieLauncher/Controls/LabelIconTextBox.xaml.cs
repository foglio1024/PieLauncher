using System.Windows;
using System.Windows.Media;

namespace PieLauncher.Controls;

public partial class LabelIconTextBox
{
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(LabelIconTextBox), new PropertyMetadata(""));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(LabelIconTextBox), new PropertyMetadata(""));

    public PathGeometry SvgIcon
    {
        get => (PathGeometry)GetValue(SvgIconProperty);
        set => SetValue(SvgIconProperty, value);
    }

    public static readonly DependencyProperty SvgIconProperty = DependencyProperty.Register(nameof(SvgIcon), typeof(PathGeometry), typeof(LabelIconTextBox), new PropertyMetadata(null));

    public LabelIconTextBox()
    {
        InitializeComponent();
    }
}