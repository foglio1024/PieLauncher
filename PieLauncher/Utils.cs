using Nostrum.WinAPI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace PieLauncher
{
    public class Utils
    {
        public static IntPtr SetHook(User32.LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule? curModule = curProcess.MainModule;
            if (curModule?.ModuleName == null) throw new InvalidOperationException();
            var hMod = Kernel32.GetModuleHandle(curModule.ModuleName);
            return User32.SetWindowsHookEx((int)User32.WindowsHooks.WH_KEYBOARD_LL, proc, hMod, 0);
        }

        public static void HideWindowFromToolBar(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle | (int)User32.ExtendedWindowStyles.WS_EX_TOOLWINDOW);
        }

        public static void MakeWindowUnfocusable(IntPtr hwnd)
        {
            var extendedStyle = User32.GetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE);
            User32.SetWindowLong(hwnd, (int)User32.GWL.GWL_EXSTYLE, extendedStyle | (int)User32.ExtendedWindowStyles.WS_EX_NOACTIVATE);
        }

        public static ScaleTransform GetOrCreateTransform(FrameworkElement elem)
        {
            var xform = new ScaleTransform();
            if (elem.RenderTransform is not ScaleTransform sc)
            {
                elem.RenderTransform = xform;
            }
            else if (elem.RenderTransform.IsFrozen || elem.RenderTransform.IsSealed)
            {
                var currScaleX = sc.ScaleX;
                var currScaleY = sc.ScaleY;
                xform = new ScaleTransform(currScaleX, currScaleY);
                elem.RenderTransform = xform;
            }
            else
            {
                xform = sc;
            }
            return xform;
        }

    }
}
