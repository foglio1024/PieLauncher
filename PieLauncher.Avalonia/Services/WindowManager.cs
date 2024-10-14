using PieLauncher.Avalonia.Views;
using PieLauncher.Core.Services;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Avalonia;

public class WindowManager : IWindowManager
{
    private ConfigWindow? _win;

    public void ShowConfigWindow(MainViewModel vm)
    {
        // todo: better send a message from here
        vm.ForceVisible = true;
        vm.Topmost = false;

        if (_win == null)
        {
            _win = new ConfigWindow { DataContext = vm };
            _win.Closed += (_, _) =>
            {
                _win = null;
                vm.ForceVisible = false;
                vm.Topmost = true;
            };
        }
        _win.Show();
    }

    public void CloseConfigWindow()
    {
        _win?.Close();
        //_win = null;
    }
}