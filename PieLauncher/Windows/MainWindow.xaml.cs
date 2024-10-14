using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.Messaging;
using Nostrum.WPF.Factories;
using PieLauncher.Core.Messages;

namespace PieLauncher.Windows;

public partial class MainWindow : 
    IRecipient<ShowHideWindowMessage>
{
    private readonly DoubleAnimation _fadeIn;
    private readonly DoubleAnimation _fadeOut;

    public MainWindow()
    {
        _fadeIn = AnimationFactory.CreateDoubleAnimation(250, to: 1, easing: true, completed: OnFadeInCompleted);
        _fadeOut = AnimationFactory.CreateDoubleAnimation(250, to: 0, easing: true, completed: OnFadeOutCompleted);

        InitializeComponent();

        Opacity = 0;
        IsHitTestVisible = false;

        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    
    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
    }

    private void OnFadeInCompleted(object? sender, EventArgs e)
    {
        IsHitTestVisible = true;
    }

    private void OnFadeOutCompleted(object? sender, EventArgs e)
    {
        Hide();
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        //return;
        //_dc.CurrentTime = _dc.CurrentTime.AddHours(e.Key is System.Windows.Input.Key.A ? -1 : e.Key is System.Windows.Input.Key.D ? 1 : 0);
        //_dc.CurrentTime = _dc.CurrentTime.AddMinutes(e.Key is System.Windows.Input.Key.W ? 1 : e.Key is System.Windows.Input.Key.S ? -1 : 0);
    }

    public void Receive(ShowHideWindowMessage message)
    {
        if (message.IsVisible)
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