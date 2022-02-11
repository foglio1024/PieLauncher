using Nostrum.WinAPI;
using Nostrum.WPF.Factories;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PieLauncher
{

    public partial class MainWindow : Window
    {
        readonly DoubleAnimation _fadeIn;
        readonly DoubleAnimation _fadeOut;
        readonly DoubleAnimation _expandButton;
        readonly DoubleAnimation _shrinkButton;

        readonly MainViewModel _dc;

        public MainWindow()
        {
            _fadeIn = AnimationFactory.CreateDoubleAnimation(150, to: 1, easing: true);
            _fadeOut = AnimationFactory.CreateDoubleAnimation(150, to: 0, easing: true, completed: OnFadeOutCompleted);
            _expandButton = AnimationFactory.CreateDoubleAnimation(150, to: 1.1, from: 1.0, easing: true);
            _shrinkButton = AnimationFactory.CreateDoubleAnimation(250, to: 1.0, from: 1.1, easing: true);

            _dc = new MainViewModel();
            this.DataContext = _dc;
            _dc.PropertyChanged += OnDataContextPropertyChanged;

            Opacity = 0;

            InitializeComponent();
        }

        void OnDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsVisible))
            {
                if (_dc.IsVisible)
                {
                    Show();
                    BeginAnimation(OpacityProperty, _fadeIn);
                    ((RotateTransform)MainContainer.RenderTransform).BeginAnimation(RotateTransform.AngleProperty,
                        AnimationFactory.CreateDoubleAnimation(300, from: -10, to: 0, easing: true));
                }
                else
                {
                    BeginAnimation(OpacityProperty, _fadeOut);
                    ((RotateTransform)MainContainer.RenderTransform).BeginAnimation(RotateTransform.AngleProperty,
                         AnimationFactory.CreateDoubleAnimation(200, from: 0, to: -10, easing: true));

                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void OnFadeOutCompleted(object? sender, EventArgs e)
        {
            Hide();
        }

        void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = (Button)sender;
            var xform = Utils.GetOrCreateTransform(btn);
            xform.BeginAnimation(ScaleTransform.ScaleXProperty, _expandButton);
            xform.BeginAnimation(ScaleTransform.ScaleYProperty, _expandButton);
        }

        void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = (Button)sender;
            var xform = Utils.GetOrCreateTransform(btn);
            xform.BeginAnimation(ScaleTransform.ScaleXProperty, _shrinkButton);
            xform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkButton);
        }
    }
}
