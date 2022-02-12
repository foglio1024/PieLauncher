using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Keys = System.Windows.Forms.Keys;

namespace PieLauncher.Controls
{
    public partial class HotkeySettingControl : UserControl
    {
        readonly List<Key> _pressedKeys;

        public HotKey Value
        {
            get => (HotKey)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(HotKey), typeof(HotkeySettingControl));

        public HotkeySettingControl()
        {
            _pressedKeys = new List<Key>();

            InitializeComponent();

            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            TheTextBox.Text = Value.ToString();
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var k = e.Key == Key.System ? Key.LeftAlt : e.Key;
            if (_pressedKeys.Contains(k)                            // key is already pressed (can this even happen?)
            || (k == Key.Tab && _pressedKeys.Contains(Key.LeftAlt)) // avoid triggering "Alt + Tab"
            ) return;

            if (e.Key == Key.Enter)
            {
                _pressedKeys.Clear();
                Keyboard.ClearFocus();
                return;
            }
            _pressedKeys.Add(k);
            UpdateValue();

        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var k = e.Key;
            if (k == Key.System) k = Key.LeftAlt;
            if (e.Key == Key.Enter)
            {
                _pressedKeys.Clear();
                Keyboard.ClearFocus();
                return;
            }
            _pressedKeys.Remove(k);
            UpdateValue();

        }

        void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            KeyboardHook.Instance.Disable();
        }

        void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            KeyboardHook.Instance.Enable();
        }

        void UpdateValue()
        {
            var shift = _pressedKeys.Contains(Key.LeftShift);
            var alt = _pressedKeys.Contains(Key.LeftAlt);
            var ctrl = _pressedKeys.Contains(Key.LeftCtrl);
            var win = _pressedKeys.Contains(Key.LWin);
            var key = _pressedKeys.FirstOrDefault(x => x != Key.LeftAlt && x != Key.LeftShift && x != Key.LeftCtrl && x != Key.LWin);
            var mod = (ctrl ? ModifierKeys.Control : ModifierKeys.None) | (win ? ModifierKeys.Windows: ModifierKeys.None) | (shift ? ModifierKeys.Shift : ModifierKeys.None) | (alt ? ModifierKeys.Alt : ModifierKeys.None);

            Enum.TryParse<Keys>(key.ToString(), out var wfKey);
            if (wfKey == Keys.None) return;
            Value = new HotKey(wfKey, mod);
            TheTextBox.Text = Value.ToString();
        }
    }
}
