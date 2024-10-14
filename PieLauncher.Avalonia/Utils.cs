using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace PieLauncher.Avalonia;

internal static class Utils
{
    internal static void MakeOverlay(HWND hwnd)
    {
        var style = PInvoke.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE,
            style
            | (int)WINDOW_STYLES.WS_DISABLED
            | (int)WINDOW_STYLES.WS_EX_TOOLWINDOW
            | (int)WINDOW_STYLES.WS_SYSMENU
        );
    }
}