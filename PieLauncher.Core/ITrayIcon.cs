namespace PieLauncher.Core;

public interface ITrayIcon : IDisposable
{
    event Action? Clicked;
    event Action? DoubleClicked;
}