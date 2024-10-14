using Nostrum.WPF.Factories;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.Messaging;
using PieLauncher.Core;
using PieLauncher.Core.Messages;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Views;

public partial class PieView : IRecipient<ShowHideWindowMessage>
{
    private MainViewModel? _dc;
    private MainViewModel DC => _dc ??= (MainViewModel)DataContext;

    private readonly RotateTransform _mainContainerTransform;
    private readonly DoubleAnimation _rollIn;
    private readonly DoubleAnimation _rollOut;
    private readonly DoubleAnimation _expandButton;
    private readonly DoubleAnimation _shrinkButton;

    public PieView()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.RegisterAll(this);

        Loaded += OnLoaded;
        _mainContainerTransform = ((RotateTransform)MainContainer.RenderTransform);
        _rollIn = AnimationFactory.CreateDoubleAnimation(250, from: -10, to: 0, easing: true);
        _rollOut = AnimationFactory.CreateDoubleAnimation(250, from: 0, to: -10, easing: true);
        _expandButton = AnimationFactory.CreateDoubleAnimation(800, to: 1.05, from: 1.0, easing: false);
        _shrinkButton = AnimationFactory.CreateDoubleAnimation(250, to: 1.0, from: 1.05, easing: true);
        _expandButton.EasingFunction = new ElasticEase();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ClockText.RenderTransform = new TranslateTransform { Y = DC.MediaInfo.IsEmpty ? 60 : 0 };
    }

    private void OnShortcutMouseEnter(object sender, MouseEventArgs e)
    {
        var btn = (FrameworkElement)sender;
        var xform = Utils.GetOrCreateScaleTransform(btn);
        xform.BeginAnimation(ScaleTransform.ScaleXProperty, _expandButton);
        xform.BeginAnimation(ScaleTransform.ScaleYProperty, _expandButton);
        var dc = (ShortcutViewModel)btn.DataContext;
        dc.IsHovered = true;
    }

    private void OnShortcutMouseLeave(object sender, MouseEventArgs e)
    {
        var btn = (FrameworkElement)sender;
        var xform = Utils.GetOrCreateScaleTransform(btn);
        xform.BeginAnimation(ScaleTransform.ScaleXProperty, _shrinkButton);
        xform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkButton);

        var dc = (ShortcutViewModel)btn.DataContext;
        dc.IsHovered = false;
    }

    public void Receive(ShowHideWindowMessage message)
    {
        _mainContainerTransform.BeginAnimation(RotateTransform.AngleProperty, message.IsVisible ? _rollIn : _rollOut);
    }
}