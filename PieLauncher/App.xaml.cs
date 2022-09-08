using Nostrum.WPF;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace PieLauncher
{
    public enum Theme
    {
        Dark,
        Light
    }
    // todo: mutex
    public partial class App : Application
    {
        readonly NotifyIcon _tray = new() { Visible = true };

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            _tray.Icon = MiscUtils.GetEmbeddedIcon("icon.ico");
            _tray.MouseClick += OnTrayClick;
            _tray.MouseDoubleClick += OnTrayDoubleClick;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            base.OnStartup(e);
        }

        public App()
        {
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
            var dc = ((MainViewModel)((MainWindow)MainWindow).DataContext);
            if (dc.ForceVisible) return;
            dc.IsVisible = !dc.IsVisible; //.OpenConfigWindowCommand.Execute(null);
        }

        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "error.txt"), e.ExceptionObject.ToString());
            System.Windows.MessageBox.Show(e.ExceptionObject.ToString(), "Pie launcher", MessageBoxButton.OK);
            App.Current.Shutdown();
        }

        public void ApplyTheme(Theme t)
        {
            return;
            var themeUri = $"Resources/{t}Theme.xaml";
            if (Current.Resources.MergedDictionaries.Any(d => d.Source.OriginalString == themeUri)) return;
            var currentThemeDict = Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.OriginalString.EndsWith("Theme.xaml"));
            if (currentThemeDict == null) return;
            Current.Resources.MergedDictionaries.Remove(currentThemeDict);
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themeUri, UriKind.Relative) });

        }
    }

}
