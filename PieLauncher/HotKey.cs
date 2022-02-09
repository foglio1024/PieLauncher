using System.Windows.Forms;
using System.Windows.Input;

namespace PieLauncher
{
#pragma warning disable CS0661 // Il tipo definisce l'operatore == o l'operatore != ma non esegue l'override di Object.GetHashCode()
    public struct HotKey
#pragma warning restore CS0661 // Il tipo definisce l'operatore == o l'operatore != ma non esegue l'override di Object.GetHashCode()
    {
        public Keys Key { get; set; }
        public ModifierKeys Modifier { get; set; }

        public HotKey(Keys k, ModifierKeys m) : this()
        {
            Key = k;
            Modifier = m;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not HotKey other) return false;
            return other.Key == Key && other.Modifier == Modifier;
        }

        public override string ToString()
        {
            var control = (Modifier & ModifierKeys.Control) != 0;
            var shift = (Modifier & ModifierKeys.Shift) != 0;
            var alt = (Modifier & ModifierKeys.Alt) != 0;
            var win = (Modifier & ModifierKeys.Windows) != 0;

            return $"{(control ? "Ctrl + " : "")}{(shift ? "Shift + " : "")}{(alt ? "Alt + " : "")}{(win ? "Win + " : "")}{Key}";
        }

        public static bool operator ==(HotKey left, HotKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HotKey left, HotKey right)
        {
            return !(left == right);
        }
    }
}
