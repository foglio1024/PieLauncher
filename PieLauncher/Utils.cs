using Microsoft.Win32;
using Nostrum.WinAPI;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PieLauncher
{
    public static class Utils
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

        public static ScaleTransform GetOrCreateScaleTransform(FrameworkElement elem)
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

        public static string BrowseIcon(string name = "")
        {
            var ofd = new OpenFileDialog
            {
                Title = $"Browse icon{(string.IsNullOrWhiteSpace(name) ? "" : $" for {name}")}",
                Filter = "Image Files(*.jpg;*.png;*.gif;*.webp;*.bmp;*.ico)|*.jpg;*.png;*.gif;*.webp;*.bmp;*.ico|Assemblies (*.exe;*.dll)|*.exe;*.dll|All files (*.*)|*.*"
            };
            ofd.ShowDialog();
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return "";
            if (ofd.FileName.EndsWith(".exe") || ofd.FileName.EndsWith(".dll"))
            {
                var sb = new StringBuilder(ofd.FileName, 256);
                if (!Shell32.SHPickIconDialog(IntPtr.Zero, sb, 256, out var idx)) return "";
                return sb.ToString() + "|" + idx;
            }
            else
            {
                return ofd.FileName;
            }
        }
        public static ImageSource? ExtractIconFromAssembly(string path, int index)
        {
            var extractor = new IconExtractor(Environment.ExpandEnvironmentVariables(path));
            var icon = extractor.GetIcon(index);
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
