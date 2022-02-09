using Newtonsoft.Json;
using Nostrum.WinAPI;
using Nostrum.WPF.Factories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Windows.Media.Control;

namespace PieLauncher
{

    public partial class MainWindow : Window
    {
        readonly DoubleAnimation _fadeIn;
        readonly DoubleAnimation _fadeOut;
        readonly DoubleAnimation _expandButton;
        readonly DoubleAnimation _shrinkButton;

        public MainWindow()
        {
            _fadeIn = AnimationFactory.CreateDoubleAnimation(150, to: 1, easing: true);
            _fadeOut = AnimationFactory.CreateDoubleAnimation(150, to: 0, easing: true, completed: OnFadeOutCompleted);
            _expandButton = AnimationFactory.CreateDoubleAnimation(150, to: 1.1, from: 1.0, easing: true);
            _shrinkButton = AnimationFactory.CreateDoubleAnimation(250, to: 1.0, from: 1.1, easing: true);

            this.DataContext = new MainViewModel
            {
                
            };

            InitializeComponent();
            Opacity = 0;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            FadeOut();
        }

        void OnFadeOutCompleted(object? sender, EventArgs e)
        {
            Hide();
        }

        public void FadeIn()
        {
            //PlaceWindow();
            Show();
            BeginAnimation(OpacityProperty, _fadeIn);
        }

        void PlaceWindow()
        {
            User32.GetCursorPos(out var p);
            Debug.WriteLine("{0} {1}", p.X, p.Y);
            PresentationSource source = PresentationSource.FromVisual(this);

            double dpiX = 1, dpiY = 1;
            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }
            this.Top = p.Y / dpiY - this.Height / 2;
            this.Left = p.X / dpiX - this.Width / 2;
        }

        internal void FadeOut()
        {
            BeginAnimation(OpacityProperty, _fadeOut);
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
