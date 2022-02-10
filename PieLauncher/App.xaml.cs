#if !DEBUG
using Microsoft.Win32;
#endif
using Nostrum.WinAPI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace PieLauncher
{
    // todo: mutex
    public partial class App : System.Windows.Application
    {
        IntPtr _hookID = IntPtr.Zero;
        readonly NotifyIcon _tray = new() { Visible = true };
        User32.LowLevelKeyboardProc? _callback;
        bool _keyDown = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            SetStartWithWindows();

            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Hide();

            var bmp = new Bitmap(@".\donut.png");
            var hIcon = bmp.GetHicon();
            _tray.Icon = Icon.FromHandle(hIcon);
            _tray.MouseClick += OnTrayClick;
            
            _callback = HookCallback; // needed to avoid being GC'd
            _hookID = Utils.SetHook(_callback);

            KeyboardHook.Instance.RegisterCallback(new HotKey(Keys.OemBackslash, ModifierKeys.Windows), OnHotKeyPressed);
            KeyboardHook.Instance.Enable();
            if(!Debugger.IsAttached) AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            User32.UnhookWindowsHookEx(_hookID);
            KeyboardHook.Instance.Dispose();
            _tray?.Dispose();

            base.OnExit(e);
        }

        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show(e.ToString(), "Pie launcher", MessageBoxButton.OK);
            App.Current.Shutdown();
        }

        void OnHotKeyPressed()
        {
            if (((MainWindow)MainWindow).IsVisible) return;
            ((MainWindow)MainWindow).FadeIn();
        }

        void SetStartWithWindows()
        {
#if !DEBUG
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            var path = Process.GetCurrentProcess().MainModule?.FileName;
            rk?.SetValue(nameof(PieLauncher), path!);
            //rk?.DeleteValue(nameof(PieLauncher), false);
#endif
        }

        void OnTrayClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            App.Current.Shutdown();
        }

        IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var nextCallback = User32.CallNextHookEx(_hookID, nCode, wParam, lParam);
            if (nCode < 0) return nextCallback;

            var key = (Keys)(Marshal.ReadInt32(lParam));
            var msg = (User32.WindowsMessages)wParam;

            if (key == Keys.OemBackslash)
            {
                if (msg == User32.WindowsMessages.WM_KEYDOWN && Keyboard.IsKeyDown(Key.LWin))
                {
                    _keyDown = true;
                }
                else if (msg == User32.WindowsMessages.WM_KEYUP && _keyDown)
                {
                    _keyDown = false;
                    ((MainWindow)MainWindow).FadeOut();
                }
            }
            return nextCallback;
        }
    }
}
