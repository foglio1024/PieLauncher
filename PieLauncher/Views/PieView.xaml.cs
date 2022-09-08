using Nostrum.WPF.Factories;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PieLauncher.Views
{
    public partial class PieView : UserControl
    {
        MainViewModel? _dc;
        MainViewModel DC => _dc ??= (MainViewModel)DataContext;

        readonly RotateTransform _mainContainerTransform;
        readonly DoubleAnimation _rollIn;
        readonly DoubleAnimation _rollOut;
        readonly DoubleAnimation _expandButton;
        readonly DoubleAnimation _shrinkButton;

        public PieView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _mainContainerTransform = ((RotateTransform)MainContainer.RenderTransform);
            _rollIn = AnimationFactory.CreateDoubleAnimation(250, from: -10, to: 0, easing: true);
            _rollOut = AnimationFactory.CreateDoubleAnimation(250, from: 0, to: -10, easing: true);
            _expandButton = AnimationFactory.CreateDoubleAnimation(800, to: 1.05, from: 1.0, easing: false);
            _shrinkButton = AnimationFactory.CreateDoubleAnimation(250, to: 1.0, from: 1.05, easing: true);
            _expandButton.EasingFunction = new ElasticEase();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            DC.PropertyChanged += OnDataContextPropertyChanged;

            ClockText.RenderTransform = new TranslateTransform { Y = DC.MediaInfo.IsEmpty ? 60 : 0 };
        }



        void OnDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsVisible))
                _mainContainerTransform.BeginAnimation(RotateTransform.AngleProperty, DC.IsVisible ? _rollIn : _rollOut);
        }

        void OnShortcutMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = (FrameworkElement)sender;
            var xform = Utils.GetOrCreateScaleTransform(btn);
            xform.BeginAnimation(ScaleTransform.ScaleXProperty, _expandButton);
            xform.BeginAnimation(ScaleTransform.ScaleYProperty, _expandButton);
            var dc = (ShortcutViewModel)btn.DataContext;
            dc.IsHovered = true;
        }

        void OnShortcutMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = (FrameworkElement)sender;
            var xform = Utils.GetOrCreateScaleTransform(btn);
            xform.BeginAnimation(ScaleTransform.ScaleXProperty, _shrinkButton);
            xform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkButton);

            var dc = (ShortcutViewModel)btn.DataContext;
            dc.IsHovered = false;
        }

    }
}
