using System.Windows.Input;

namespace PieLauncher.Core;

public struct HotKey
{
    public Keys Key { get; set; } = Keys.OemBackslash;
    public ModifierKeys Modifier { get; set; } = ModifierKeys.Windows;
    private readonly bool Ctrl => (Modifier & ModifierKeys.Control) != 0;
    private readonly bool Shift => (Modifier & ModifierKeys.Shift) != 0;
    private readonly bool Alt => (Modifier & ModifierKeys.Alt) != 0;
    private readonly bool Win => (Modifier & ModifierKeys.Windows) != 0;

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

    private readonly bool Equals(HotKey other)
    {
        return Key == other.Key && Modifier == other.Modifier;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is HotKey other && Equals(other);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine((int)Key, (int)Modifier);
    }

    public readonly override string ToString()
    {
        return $"{(Ctrl ? "Ctrl + " : "")}{(Shift ? "Shift + " : "")}{(Alt ? "Alt + " : "")}{(Win ? "Win + " : "")}{Key}"
            .Replace("OemBackslash", "<");
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