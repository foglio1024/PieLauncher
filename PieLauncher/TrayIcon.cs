using Nostrum.WPF;
using PieLauncher.Core;
using System;
using System.Windows.Forms;

namespace PieLauncher;

public class TrayIcon : ITrayIcon
{
    private readonly NotifyIcon _tray ;

    public event Action? Clicked;
    public event Action? DoubleClicked;

    public TrayIcon()
    {
        _tray = new NotifyIcon
        {
            Visible = true,
            Icon = MiscUtils.GetEmbeddedIcon("icon.ico"),
        };

        _tray.MouseClick += OnTrayClick;
        _tray.MouseDoubleClick += OnTrayDoubleClick;

    }

    public void Dispose()
    {
        _tray.MouseClick -= OnTrayClick;
        _tray.MouseDoubleClick -= OnTrayDoubleClick;

        _tray.Dispose();
    }

    private void OnTrayDoubleClick(object? sender, MouseEventArgs e)
    {
        DoubleClicked?.Invoke();
    }

    private void OnTrayClick(object? sender, EventArgs e)
    {
        Clicked?.Invoke();
    }
}