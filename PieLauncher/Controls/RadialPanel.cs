using System;
using System.Windows;
using System.Windows.Controls;
using Nostrum.Extensions;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace PieLauncher.Controls;

public class RadialPanel : Panel
{
    public double AngleOffset
    {
        get => (double)GetValue(AngleOffsetProperty);
        set => SetValue(AngleOffsetProperty, value);
    }
    public static readonly DependencyProperty AngleOffsetProperty = DependencyProperty.Register(nameof(AngleOffset), typeof(double), typeof(RadialPanel), new PropertyMetadata(0d, ForceInvalidateVisual));


    public SwipeDirection SwipeDirection
    {
        get => (SwipeDirection)GetValue(SwipeDirectionProperty);
        set => SetValue(SwipeDirectionProperty, value);
    }
    public static readonly DependencyProperty SwipeDirectionProperty = DependencyProperty.Register(nameof(SwipeDirection), typeof(SwipeDirection), typeof(RadialPanel), new PropertyMetadata(SwipeDirection.Clockwise, ForceInvalidateVisual));

    public double TotalAngle
    {
        get => (double)GetValue(TotalAngleProperty);
        set => SetValue(TotalAngleProperty, value);
    }
    public static readonly DependencyProperty TotalAngleProperty = DependencyProperty.Register(nameof(TotalAngle), typeof(double), typeof(RadialPanel), new PropertyMetadata(360d, ForceInvalidateVisual));

    private static void ForceInvalidateVisual(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
            if (d is not RadialPanel rp) return;
            rp.InvalidateVisual();
        }

    // Measure each children and give as much room as they want 
    protected override Size MeasureOverride(Size availableSize)
    {
            foreach (UIElement elem in Children)
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

            foreach (UIElement elem in Children)
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