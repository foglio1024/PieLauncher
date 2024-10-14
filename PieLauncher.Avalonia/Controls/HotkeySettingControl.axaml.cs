using Avalonia.Controls;
using PieLauncher.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Key = Avalonia.Input.Key;
using KeyEventArgs = Avalonia.Input.KeyEventArgs;
using UserControl = Avalonia.Controls.UserControl;

namespace PieLauncher.Avalonia.Controls;

public partial class HotkeySettingControl : UserControl
{
    private readonly List<Key> _pressedKeys = [];

    public HotKey Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<HotKey> ValueProperty = AvaloniaProperty.Register<HotkeySettingControl, HotKey>(nameof(Value));

    public HotkeySettingControl()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        TheTextBox.Text = Value.ToString();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;
        var k = e.Key == Key.System ? Key.LeftAlt : e.Key;
        if (_pressedKeys.Contains(k)                            // key is already pressed (can this even happen?)
            || (k == Key.Tab && _pressedKeys.Contains(Key.LeftAlt)) // avoid triggering "Alt + Tab"
           ) return;

        if (e.Key == Key.Enter)
        {
            _pressedKeys.Clear();
            TopLevel.GetTopLevel(this)?.Focus();
            //Keyboard.ClearFocus();
            return;
        }
        _pressedKeys.Add(k);
        UpdateValue();

    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        e.Handled = true;
        var k = e.Key;
        if (k == Key.System) k = Key.LeftAlt;
        if (e.Key == Key.Enter)
        {
            _pressedKeys.Clear();
            TopLevel.GetTopLevel(this)?.Focus();
            //Keyboard.ClearFocus();
            return;
        }
        _pressedKeys.Remove(k);
        UpdateValue();

    }

    private void OnGotKeyboardFocus(object sender, GotFocusEventArgs e)
    {
        KeyboardHook.Instance.Disable();
    }

    private void OnLostKeyboardFocus(object? sender, RoutedEventArgs e)
    {
        KeyboardHook.Instance.Enable();
    }

    private void UpdateValue()
    {
        var shift = _pressedKeys.Contains(Key.LeftShift);
        var alt = _pressedKeys.Contains(Key.LeftAlt);
        var ctrl = _pressedKeys.Contains(Key.LeftCtrl);
        var win = _pressedKeys.Contains(Key.LWin);
        var key = _pressedKeys.FirstOrDefault(x => x is not Key.LeftAlt and not Key.LeftShift and not Key.LeftCtrl and not Key.LWin);
        var mod = (ctrl ? ModifierKeys.Control : ModifierKeys.None)
                  | (win ? ModifierKeys.Windows : ModifierKeys.None)
                  | (shift ? ModifierKeys.Shift : ModifierKeys.None)
                  | (alt ? ModifierKeys.Alt : ModifierKeys.None);

        Enum.TryParse<Keys>(key.ToString(), out var wfKey);
        if (wfKey == Keys.None) return;
        Value = new HotKey(wfKey, mod);
        TheTextBox.Text = Value.ToString();
    }
    
}
