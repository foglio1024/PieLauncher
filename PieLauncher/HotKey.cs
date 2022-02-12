using System.Collections.Generic;
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

        public List<Key> ModifierList
        {
            get
            {
                var ret = new List<Key>();
                if (Ctrl) ret.Add(System.Windows.Input.Key.LeftCtrl);
                if (Ctrl) ret.Add(System.Windows.Input.Key.RightCtrl);
                if (Shift) ret.Add(System.Windows.Input.Key.LeftShift);
                if (Shift) ret.Add(System.Windows.Input.Key.RightShift);
                if (Alt) ret.Add(System.Windows.Input.Key.LeftAlt);
                if (Alt) ret.Add(System.Windows.Input.Key.RightAlt);
                if (Win) ret.Add(System.Windows.Input.Key.LWin);
                if (Win) ret.Add(System.Windows.Input.Key.RWin);
                return ret;
            }
        }
        public HotKey(Keys k, ModifierKeys m) : this()
        {
            Key = k;
            Modifier = m;
        }

        public bool Ctrl => (Modifier & ModifierKeys.Control) != 0;
        public bool Shift => (Modifier & ModifierKeys.Shift) != 0;
        public bool Alt => (Modifier & ModifierKeys.Alt) != 0;
        public bool Win => (Modifier & ModifierKeys.Windows) != 0;

        public override bool Equals(object? obj)
        {
            if (obj is not HotKey other) return false;
            return other.Key == Key && other.Modifier == Modifier;
        }

        public override string ToString()
        {
            return $"{(Ctrl ? "Ctrl + " : "")}{(Shift ? "Shift + " : "")}{(Alt ? "Alt + " : "")}{(Win ? "Win + " : "")}{Key}";
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
