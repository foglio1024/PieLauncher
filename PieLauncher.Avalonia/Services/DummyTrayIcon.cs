using System;
using PieLauncher.Core;

namespace PieLauncher.Avalonia.Services;

internal class DummyTrayIcon : ITrayIcon
{
    public event Action? Clicked;

    public event Action? DoubleClicked;

    public void Dispose()
    {
    }
}