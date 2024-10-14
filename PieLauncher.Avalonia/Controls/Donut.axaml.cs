using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PieLauncher.Avalonia.Controls;

public partial class Donut : UserControl
{
    private readonly DropShadowEffect _ringShadow = new() { OffsetX = 0, OffsetY = 0, BlurRadius = 100, Opacity = .7 };

    public double InnerRadius
    {
        get => GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    public static readonly StyledProperty<double> InnerRadiusProperty =
        AvaloniaProperty.Register<Donut, double>(nameof(InnerRadius), 60d);

    public double OuterRadius
    {
        get => GetValue(OuterRadiusProperty);
        set => SetValue(OuterRadiusProperty, value);
    }

    public static readonly StyledProperty<double> OuterRadiusProperty =
        AvaloniaProperty.Register<Donut, double>(nameof(OuterRadius), 100d);

    public Brush Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly StyledProperty<Brush> FillProperty =
        AvaloniaProperty.Register<Donut, Brush>(nameof(Fill), new SolidColorBrush(Colors.SlateGray));

    public Brush Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public static readonly StyledProperty<Brush> StrokeProperty =
        AvaloniaProperty.Register<Donut, Brush>(nameof(Stroke), new SolidColorBrush(Colors.LightSlateGray));

    public bool HasShadow
    {
        get => this.GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }

    public static readonly StyledProperty<bool> HasShadowProperty =
        AvaloniaProperty.Register<Donut, bool>(nameof(HasShadow), true);

    private void OnHasShadowChanged(Donut donut, AvaloniaPropertyChangedEventArgs e)
    {
        MainRing.Effect = HasShadow
            ? _ringShadow
            : null;
    }

    public Donut()
    {
        InitializeComponent();

        OuterRadiusProperty.Changed.AddClassHandler<Donut>(OnRadiiChanged);
        InnerRadiusProperty.Changed.AddClassHandler<Donut>(OnRadiiChanged);
        HasShadowProperty.Changed.AddClassHandler<Donut>(OnHasShadowChanged);

        OnHasShadowChanged(this, null!);
    }

    private void OnRadiiChanged(Donut donut, AvaloniaPropertyChangedEventArgs e)
    {
        MainRing.StrokeThickness = Math.Abs(OuterRadius - InnerRadius) / 2;
    }
}