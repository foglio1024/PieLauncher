using Nostrum.WPF.Factories;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PieLauncher.Controls
{
    public partial class SeekBar : UserControl
    {
        public double Completion
        {
            get { return (double)GetValue(CompletionProperty); }
            set { SetValue(CompletionProperty, value); }
        }
        public static readonly DependencyProperty CompletionProperty = DependencyProperty.Register("Completion", typeof(double), typeof(SeekBar), new PropertyMetadata(0d, CompletionChangedCallback));

        static void CompletionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SeekBar sb) sb.OnCompletionChanged((double)e.OldValue, (double)e.NewValue);
        }

        void OnCompletionChanged(double oldValue, double newValue)
        {
            var bgW = BarBackground.ActualWidth;
            if(double.IsNaN(oldValue) || double.IsNaN(newValue))
            {
                BarForeground.Width = 0;
                return;
            }
            var newWidth = newValue * bgW;
            var oldWidth = oldValue * bgW;
            BarForeground.BeginAnimation(WidthProperty, AnimationFactory.CreateDoubleAnimation(250, from: oldWidth, to: newWidth, easing: true));
        }

        public SeekBar()
        {
            InitializeComponent();
        }
    }
}
