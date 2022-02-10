using System.Windows.Input;

namespace PieLauncher
{
    public interface IPieItem
    {
        string Name { get; set; }
        ICommand DeleteCommand { get; }
    }
}