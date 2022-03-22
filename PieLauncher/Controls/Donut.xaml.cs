using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PieLauncher.Controls
{
    public partial class Donut : UserControl
    {
        readonly DropShadowEffect _ringShadow = new() { ShadowDepth = 0, BlurRadius = 100, Opacity = .7 };

        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(Donut), new PropertyMetadata(60d, HandleRadiiChanged));

        public double OuterRadius
        {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadius", typeof(double), typeof(Donut), new PropertyMetadata(100d, HandleRadiiChanged));


        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(Donut), new PropertyMetadata(Brushes.SlateGray));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(Donut), new PropertyMetadata(Brushes.LightSlateGray));

        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        public static readonly DependencyProperty HasShadowProperty =
            DependencyProperty.Register("HasShadow", typeof(bool), typeof(Donut), new PropertyMetadata(true, HandleHasShadowChanged));

        static void HandleHasShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Donut)d).OnHasShadowChanged();
        }

        void OnHasShadowChanged()
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

        void OnRadiiChanged()
        {
            MainRing.StrokeThickness = Math.Abs(OuterRadius - InnerRadius)/2;
        }


        static void HandleRadiiChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Donut)d).OnRadiiChanged();
        }

    }
}
