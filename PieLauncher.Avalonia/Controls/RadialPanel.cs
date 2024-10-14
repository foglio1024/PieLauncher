using Avalonia;
using Avalonia.Controls;
using Nostrum.Extensions;
using System;
using Point = Avalonia.Point;
using Rect = Avalonia.Rect;
using Size = Avalonia.Size;

namespace PieLauncher.Avalonia.Controls;

public class RadialPanel : Panel
{
    public double AngleOffset
    {
        get => GetValue(AngleOffsetProperty);
        set => SetValue(AngleOffsetProperty, value);
    }

    public static readonly StyledProperty<double> AngleOffsetProperty =
        AvaloniaProperty.Register<RadialPanel, double>(nameof(AngleOffset));

    public SwipeDirection SwipeDirection
    {
        get => GetValue(SwipeDirectionProperty);
        set => SetValue(SwipeDirectionProperty, value);
    }

    public static readonly StyledProperty<SwipeDirection> SwipeDirectionProperty =
        AvaloniaProperty.Register<RadialPanel, SwipeDirection>(nameof(SwipeDirection));

    public double TotalAngle
    {
        get => GetValue(TotalAngleProperty);
        set => SetValue(TotalAngleProperty, value);
    }

    public static readonly StyledProperty<double> TotalAngleProperty =
        AvaloniaProperty.Register<RadialPanel, double>(nameof(TotalAngle), 360d);

    public RadialPanel()
    {
        AngleOffsetProperty.Changed.AddClassHandler<RadialPanel>(ForceInvalidateVisual);
        SwipeDirectionProperty.Changed.AddClassHandler<RadialPanel>(ForceInvalidateVisual);
        TotalAngleProperty.Changed.AddClassHandler<RadialPanel>(ForceInvalidateVisual);
    }

    private void ForceInvalidateVisual(RadialPanel radialPanel, AvaloniaPropertyChangedEventArgs e)
    {
        InvalidateVisual();
    }

    // Measure each children and give as much room as they want
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (Control elem in Children)
        {
            //Give Infinite size as the avaiable size for all the children
            elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }
        return base.MeasureOverride(availableSize);
    }

    //Arrange all children based on the geometric equations for the circle.
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return finalSize;

        var _angle = (90d + AngleOffset).ToRad();

        //Degrees converted to Radian by multiplying with PI/180
        var actualCount = TotalAngle == 360 ? Children.Count : Children.Count - 1; // not the best approach
        var _incrementalAngularSpace = (TotalAngle / actualCount) * (Math.PI / 180);

        //An approximate radii based on the avialable size , obviusly a better approach is needed here.
        var radiusX = finalSize.Width / 2.4;
        var radiusY = finalSize.Height / 2.4;

        foreach (Control elem in Children)
        {
            //Calculate the point on the circle for the element
            Point childPoint = new Point(Math.Cos(_angle) * radiusX, -Math.Sin(_angle) * radiusY);

            //Offsetting the point to the Avalable rectangular area which is FinalSize.
            Point actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

            //Call Arrange method on the child element by giving the calculated point as the placementPoint.
            elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));

            //Calculate the new _angle for the next element
            _angle = SwipeDirection switch
            {
                SwipeDirection.Clockwise => _angle - _incrementalAngularSpace,
                _ => _angle + _incrementalAngularSpace,
            };
        }

        return finalSize;
    }
}