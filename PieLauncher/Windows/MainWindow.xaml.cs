using Nostrum.WPF.Factories;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PieLauncher
{
    public partial class MainWindow : Window
    {
        readonly DoubleAnimation _fadeIn;
        readonly DoubleAnimation _rollIn;
        readonly DoubleAnimation _fadeOut;
        readonly DoubleAnimation _rollOut;
        readonly DoubleAnimation _expandButton;
        readonly DoubleAnimation _shrinkButton;
        readonly RotateTransform _rotateTransform;

        readonly MainViewModel _dc;

        public MainWindow()
        {
            _fadeIn = AnimationFactory.CreateDoubleAnimation(250, to: 1, easing: true, completed: OnFadeInCompleted);
            _fadeOut = AnimationFactory.CreateDoubleAnimation(250, to: 0, easing: true, completed: OnFadeOutCompleted);
            _rollIn = AnimationFactory.CreateDoubleAnimation(250, from: -10, to: 0, easing: true);
            _rollOut = AnimationFactory.CreateDoubleAnimation(250, from: 0, to: -10, easing: true);
            _expandButton = AnimationFactory.CreateDoubleAnimation(150, to: 1.1, from: 1.0, easing: true);
            _shrinkButton = AnimationFactory.CreateDoubleAnimation(250, to: 1.0, from: 1.1, easing: true);

            _dc = new MainViewModel();
            _dc.PropertyChanged += OnDataContextPropertyChanged;
            DataContext = _dc;

            Loaded += OnLoaded;

            InitializeComponent();

            _rotateTransform = ((RotateTransform)MainContainer.RenderTransform);
            Opacity = 0;
            IsHitTestVisible = false;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            ClockText.RenderTransform = new TranslateTransform { Y = _dc.MediaInfo.IsEmpty ? 60 : 0 };
        }

        void OnDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsVisible))
            {
                if (_dc.IsVisible)
                {
                    Show();
                    BeginAnimation(OpacityProperty, _fadeIn);
                    _rotateTransform.BeginAnimation(RotateTransform.AngleProperty,_rollIn);
                }
                else
                {
                    PresentationSource.CurrentSources.OfType<HwndSource>()
                           .Select(h => h.RootVisual)
                           .OfType<FrameworkElement>()
                           .Select(f => f.Parent)
                           .OfType<Popup>()
                           .ToList()
                           .ForEach(popup => popup.SetCurrentValue(Popup.IsOpenProperty, false)); 
                    IsHitTestVisible = false;
                    BeginAnimation(OpacityProperty, _fadeOut);
                    _rotateTransform.BeginAnimation(RotateTransform.AngleProperty,_rollOut);

                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void OnFadeInCompleted(object? sender, EventArgs e)
        {
            IsHitTestVisible = true;
        }

        void OnFadeOutCompleted(object? sender, EventArgs e)
        {
            Hide();
        }

        void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = (Button)sender;
            var xform = Utils.GetOrCreateScaleTransform(btn);
            xform.BeginAnimation(ScaleTransform.ScaleXProperty, _expandButton);
            xform.BeginAnimation(ScaleTransform.ScaleYProperty, _expandButton);
        }

        void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = (Button)sender;
            var xform = Utils.GetOrCreateScaleTransform(btn);
            xform.BeginAnimation(ScaleTransform.ScaleXProperty, _shrinkButton);
            xform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkButton);
        }
    }
}
