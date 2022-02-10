using Newtonsoft.Json;
using System.Windows.Input;

namespace PieLauncher
{
    public class SeparatorViewModel : PieItemBase
    {
        public override string Name { get; set; } = "---";

        public override string ToString()
        {
            return Name;
        }
    }
}
