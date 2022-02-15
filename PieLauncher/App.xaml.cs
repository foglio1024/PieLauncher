using Nostrum.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace PieLauncher
{
    // todo: mutex
    public partial class App : Application
    {
        readonly NotifyIcon _tray = new() { Visible = true };

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Hide();

            _tray.Icon = MiscUtils.GetEmbeddedIcon("icon.ico");
            _tray.MouseClick += OnTrayClick;
            _tray.MouseDoubleClick += OnTrayDoubleClick;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            KeyboardHook.Instance.Dispose();
            _tray?.Dispose();

            base.OnExit(e);
        }
        void OnTrayDoubleClick(object? sender, MouseEventArgs e)
        {
            App.Current.Shutdown();
        }

        void OnTrayClick(object? sender, EventArgs e)
        {
            ((MainViewModel)((MainWindow)MainWindow).DataContext).OpenConfigWindowCommand.Execute(null);
        }

        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "error.txt"), e.ExceptionObject.ToString());
            System.Windows.MessageBox.Show(e.ExceptionObject.ToString(), "Pie launcher", MessageBoxButton.OK);
            App.Current.Shutdown();
        }

    }
}
