using PieLauncher.Core.ViewModels;

namespace PieLauncher.Core.Services;

public interface IWindowManager
{
    void ShowConfigWindow(MainViewModel vm);
    void CloseConfigWindow();
}