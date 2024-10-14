using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using PieLauncher.Core;
using System;

namespace PieLauncher.Avalonia;

using global::Avalonia.Platform;

internal class AvaloniaTrayIcon : ITrayIcon
{
    private readonly TrayIcon _trayIcon;

    public event Action? Clicked;

    public event Action? DoubleClicked;

    public AvaloniaTrayIcon()
    {
        _trayIcon = new TrayIcon()
        {
            ToolTipText = "PieLauncher",
            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://PieLauncher.Avalonia/Assets/icon.ico"))),
            Command = new RelayCommand(() => Clicked?.Invoke()),
            Menu = 
            [
                new NativeMenuItem("Close")
                {
                    Command = new RelayCommand(() => DoubleClicked?.Invoke())
                }
            ]
        };
    }

    public void Dispose()
    {
        _trayIcon.Dispose();
    }
}