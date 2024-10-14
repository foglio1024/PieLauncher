using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PieLauncher.Controls;

public partial class Donut
{
    private readonly DropShadowEffect _ringShadow = new() { ShadowDepth = 0, BlurRadius = 100, Opacity = .7 };

    public double InnerRadius
    {
        get => (double)GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    public static readonly DependencyProperty InnerRadiusProperty =
        DependencyProperty.Register(nameof(InnerRadius), typeof(double), typeof(Donut), new PropertyMetadata(60d, HandleRadiiChanged));

    public double OuterRadius
    {
        get => (double)GetValue(OuterRadiusProperty);
        set => SetValue(OuterRadiusProperty, value);
    }

    public static readonly DependencyProperty OuterRadiusProperty =
        DependencyProperty.Register(nameof(OuterRadius), typeof(double), typeof(Donut), new PropertyMetadata(100d, HandleRadiiChanged));

    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(Donut), new FrameworkPropertyMetadata(Brushes.SlateGray, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(Donut), new FrameworkPropertyMetadata(Brushes.LightSlateGray, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public bool HasShadow
    {
        get => (bool)GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }

    public static readonly DependencyProperty HasShadowProperty =
        DependencyProperty.Register(nameof(HasShadow), typeof(bool), typeof(Donut), new PropertyMetadata(true, HandleHasShadowChanged));

    private static void HandleHasShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Donut)d).OnHasShadowChanged();
    }

    private void OnHasShadowChanged()
    {
        MainRing.Effect = HasShadow
            ? _ringShadow
            : null;
    }

    public Donut()
    {
        InitializeComponent();
        OnHasShadowChanged();
    }

    private void OnRadiiChanged()
    {
        MainRing.StrokeThickness = Math.Abs(OuterRadius - InnerRadius) / 2;
    }

    private static void HandleRadiiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Donut)d).OnRadiiChanged();
    }
}