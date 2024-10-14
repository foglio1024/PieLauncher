using PieLauncher.Core.Services;
using PieLauncher.Core.ViewModels;
using PieLauncher.Windows;

namespace PieLauncher;

internal class WindowManager : IWindowManager
{
    private ConfigWindow? _win;

    public void ShowConfigWindow(MainViewModel vm)
    {
        if (_win != null) return;

        // todo: better send a message from here
        vm.ForceVisible = true;
        vm.Topmost = false;

        _win = new ConfigWindow { DataContext = vm };
        _win.Closed += (_, _) =>
        {
            _win = null;
            vm.ForceVisible = false;
            vm.Topmost = true;
        };
        _win.Show();
    }

    public void CloseConfigWindow()
    {
        _win?.Close();
        _win = null;
    }
}