using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using PieLauncher.Avalonia.Controls;
using PieLauncher.Core.Messages;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Win32.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Avalonia.Views;

public class PieLauncherWindow <TViewModel> 
    : Window where TViewModel : ObservableObject
{
    protected new TViewModel DataContext => Unsafe.As<TViewModel>(base.DataContext!);
}

public partial class MainWindow :
    PieLauncherWindow<MainViewModel>,
    IRecipient<ShowHideWindowMessage>
{
    public MainWindow()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        var hwnd = new HWND(TryGetPlatformHandle()!.Handle);
        Utils.MakeOverlay(hwnd);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        e.Cancel = true;
    }

    public async void Receive(ShowHideWindowMessage message)
    {
        if (message.IsVisible)
        {
            Show();
            Debug.WriteLine("Showing...");
        }
        else
        {
            Debug.WriteLine("Hiding...");

            FlyoutOnHoverBehavior.ForceClose();
            await Task.Delay(400);

            if(!DataContext.IsVisible)
                Hide();
        }
    }
}