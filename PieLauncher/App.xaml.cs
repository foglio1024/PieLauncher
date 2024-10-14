using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using PieLauncher.Core;
using PieLauncher.Core.Messages;
using PieLauncher.Core.Services;
using PieLauncher.Core.ViewModels;
using PieLauncher.Windows;

namespace PieLauncher;

// todo: mutex

public partial class App : IRecipient<ChangeThemeMessage>
{
    private readonly IServiceProvider _services;

    public App()
    {
        _services  = new ServiceCollection()
            .AddSingleton<ITrayIcon>(_ => new TrayIcon())
            .AddSingleton<IWindowManager>(_ => new WindowManager())
            .AddSingleton<IStorageService>(_ => new StorageService())
            .BuildServiceProvider();

        _services.GetService<ITrayIcon>()!.DoubleClicked += OnTrayDoubleClicked;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(_services);
        MainWindow.DataContext = ActivatorUtilities.CreateInstance<MainViewModel>(_services, false);

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        base.OnStartup(e);

        WeakReferenceMessenger.Default.Send(new AppReadyMessage());
    }

    protected override void OnExit(ExitEventArgs e)
    {
        KeyboardHook.Instance.Dispose();

        _services.GetService<ITrayIcon>()?.Dispose();

        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "error.txt"), e.ExceptionObject.ToString());
        MessageBox.Show(e.ExceptionObject.ToString(), "Pie launcher", MessageBoxButton.OK);
        App.Current.Shutdown();
    }

    private void OnTrayDoubleClicked()
    {
        Shutdown();
    }

    public void Receive(ChangeThemeMessage message)
    {
        //return;
        //var themeUri = $"Resources/{t}Theme.xaml";
        //if (Current.Resources.MergedDictionaries.Any(d => d.Source.OriginalString == themeUri)) return;
        //var currentThemeDict = Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.OriginalString.EndsWith("Theme.xaml"));
        //if (currentThemeDict == null) return;
        //Current.Resources.MergedDictionaries.Remove(currentThemeDict);
        //Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themeUri, UriKind.Relative) });
    }
}