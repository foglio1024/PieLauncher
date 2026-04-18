using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using PieLauncher.Core;
using System;

namespace PieLauncher.Avalonia;

using global::Avalonia.Platform;
using System.Reflection;

internal class AvaloniaTrayIcon : ITrayIcon
{
    private readonly TrayIcon _trayIcon;

    public event Action? Clicked;

    public event Action? DoubleClicked;

    public AvaloniaTrayIcon()
    {
        var version = Assembly.GetEntryAssembly().GetName().Version;
        _trayIcon = new TrayIcon()
        {
            ToolTipText = "PieLauncher " + version,
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