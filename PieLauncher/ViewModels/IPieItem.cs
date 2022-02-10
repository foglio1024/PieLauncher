using System.Windows.Input;

namespace PieLauncher
{
    public interface IPieItem
    {
        string Name { get; set; }
        ICommand DeleteCommand { get; }
        ICommand CopyCommand { get; }
        ICommand CutCommand { get; }
        ICommand PasteCommand { get; }
        ICommand MoveToRootCommand { get; }

        IPieItem Clone();
    }
}