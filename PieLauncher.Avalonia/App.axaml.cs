using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using PieLauncher.Avalonia.Services;
using PieLauncher.Avalonia.Views;
using PieLauncher.Core;
using PieLauncher.Core.Messages;
using PieLauncher.Core.Services;
using PieLauncher.Core.ViewModels;
using System;

namespace PieLauncher.Avalonia;

public class App : Application
{
    private IServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var tray = new AvaloniaTrayIcon();
            tray.DoubleClicked += () => desktop.Shutdown();

            _services = new ServiceCollection()
                .AddSingleton<ITrayIcon>(_ => tray)
                .AddSingleton<IWindowManager>(_ => new WindowManager())
                .AddSingleton<IStorageService>(_ => new StorageService(desktop))
                .AddSingleton(_ =>
                {
                    try
                    {
                        return new MediaInfoViewModel();
                    }   
                    catch (Exception)
                    {
                        return null;
                    }
                })
                .BuildServiceProvider();

            BindingPlugins.DataValidators.RemoveAt(0);

            desktop.MainWindow = ActivatorUtilities.CreateInstance<MainWindow>(_services);
            desktop.MainWindow.DataContext = ActivatorUtilities.CreateInstance<MainViewModel>(_services, Design.IsDesignMode);

            desktop.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();

        WeakReferenceMessenger.Default.Send(AppReadyMessage.Instance);
    }

    private static void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(AppExitingMessage.Instance);
    }
}




