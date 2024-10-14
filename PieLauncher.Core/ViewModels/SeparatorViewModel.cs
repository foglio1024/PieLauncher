using System.Windows.Media;

namespace PieLauncher.Core.ViewModels;

public class SeparatorViewModel : PieItemBase
{
    public override string Name { get; set; } = "---";

    public override string ToString()
    {
        return Name;
    }

    public override ImageSource? ImageSource => null;

    public override IPieItem Clone()
    {
        return new SeparatorViewModel();
    }
}