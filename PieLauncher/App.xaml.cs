#if !DEBUG
using Microsoft.Win32;
#endif
using Nostrum.Extensions;
using Nostrum.WinAPI;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace PieLauncher
{
    // todo: mutex
    public partial class App : Application
    {
        readonly NotifyIcon _tray = new() { Visible = true };

        protected override void OnStartup(StartupEventArgs e)
        {
            SetStartWithWindows();


            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Hide();

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetResourceStream("icon.ico")!;
            using var reader = new StreamReader(stream);
            reader.ReadToEnd();
            var bmp = new Bitmap(stream);
            var hIcon = bmp.GetHicon();
            _tray.Icon = Icon.FromHandle(hIcon);
            _tray.MouseClick += OnTrayClick;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            KeyboardHook.Instance.Dispose();
            _tray?.Dispose();

            base.OnExit(e);
        }

        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "error.txt"), e.ToString());
            System.Windows.MessageBox.Show(e.ToString(), "Pie launcher", MessageBoxButton.OK);
            App.Current.Shutdown();
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

    }
}
