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
        readonly DoubleAnimation _fadeOut;

        readonly MainViewModel _dc;

        public MainWindow()
        {
            _fadeIn = AnimationFactory.CreateDoubleAnimation(250, to: 1, easing: true, completed: OnFadeInCompleted);
            _fadeOut = AnimationFactory.CreateDoubleAnimation(250, to: 0, easing: true, completed: OnFadeOutCompleted);

            _dc = new MainViewModel();
            _dc.PropertyChanged += OnDataContextPropertyChanged;
            DataContext = _dc;


            InitializeComponent();

            Opacity = 0;
            IsHitTestVisible = false;

            _ = _dc.InitialShow();
        }


        void OnDataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsVisible))
            {
                if (_dc.IsVisible)
                {
                    Show();
                    BeginAnimation(OpacityProperty, _fadeIn);
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


        void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            return;
            _dc.CurrentTime = _dc.CurrentTime.AddHours(e.Key is System.Windows.Input.Key.A ? -1 : e.Key is System.Windows.Input.Key.D ? 1 : 0);
            _dc.CurrentTime = _dc.CurrentTime.AddMinutes(e.Key is System.Windows.Input.Key.W ? 1 : e.Key is System.Windows.Input.Key.S ? -1 : 0);
        }
    }
}
