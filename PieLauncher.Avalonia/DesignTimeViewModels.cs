using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using PieLauncher.Avalonia.Services;
using PieLauncher.Core;
using PieLauncher.Core.Services;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Avalonia;

public static class DesignTimeViewModels
{
    public static MainViewModel Main { get; }

    static DesignTimeViewModels()
    {
        //var mock = Environment.GetCommandLineArgs().Contains("--mock");

        var services = new ServiceCollection()
            .AddSingleton<ITrayIcon>(_ => new DummyTrayIcon())
            .AddSingleton<IWindowManager>(_ => new WindowManager())
            //.AddSingleton<IStorageService>(_ => new StorageService())
            .BuildServiceProvider();

        Main = ActivatorUtilities.CreateInstance<MainViewModel>(services, Design.IsDesignMode);
        Main.IsVisible = true;
    }
}