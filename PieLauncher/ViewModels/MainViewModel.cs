using Microsoft.Win32;
using Newtonsoft.Json;
using Nostrum.WinAPI;
using Nostrum.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PieLauncher
{
    public class ContentControlExtensions
    {


        public static string GetLabel(DependencyObject obj)
        {
            return (string)obj.GetValue(LabelProperty);
        }

        public static void SetLabel(DependencyObject obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached("Label", typeof(string), typeof(ContentControlExtensions), new PropertyMetadata(""));


    }
    public class MainViewModel : ObservableObject
    {

        public static readonly List<FolderViewModel> FolderRegistry = new();

        static readonly DateTimeFormatInfo DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        readonly DispatcherTimer _clockUpdateTimer;
        readonly User32.LowLevelKeyboardProc _callback;
        IntPtr _hookID = IntPtr.Zero;
        bool _hotkeyLocked = false;
        bool _keyDown = false;
        ConfigWindow? _configWindow;

        ShortcutViewModel? _hoveredShortcut;

        FolderViewModel _root;
        public FolderViewModel Root
        {
            get => _root;
            private set
            {
                if (_root == value) return;
                _root = value;
                N();
            }
        }

        HotKey _hotKey;
        public HotKey HotKey
        {
            get => _hotKey;
            set
            {
                if (_hotKey == value) return;
                KeyboardHook.Instance.ChangeHotkey(_hotKey, value);
                _hotKey = value;
                N();
            }
        }

        TriggerMode _triggerMode;
        public TriggerMode TriggerMode
        {
            get => _triggerMode;
            set
            {
                if (_triggerMode == value) return;
                _triggerMode = value;
                N();
            }
        }

        Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set
            {
                if (_theme == value) return;
                _theme = value;
                ((App)System.Windows.Application.Current).ApplyTheme(value);
                N();
            }
        }

        bool _closeAfterClick;
        public bool CloseAfterClick
        {
            get => _closeAfterClick;
            set
            {
                if (_closeAfterClick == value) return;
                _closeAfterClick = value;
                N();
            }
        }

        bool _topmost = true;
        public bool Topmost
        {
            get => _topmost;
            set
            {
                if (_topmost == value) return;
                _topmost = value;
                N();
            }
        }


        bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                N();
            }
        }

        bool _forceVisible;
        public bool ForceVisible
        {
            get => _forceVisible;
            set
            {
                if (_forceVisible == value) return;
                _forceVisible = value;
            }
        }

        bool _startWithWindows;
        public bool StartWithWindows
        {
            get => _startWithWindows;
            set
            {
                if (_startWithWindows == value) return;
                _startWithWindows = value;

                N();
            }
        }


        public MediaInfoViewModel MediaInfo { get; set; } = new();

        DateTime _currentTime = DateTime.Now;
        public DateTime CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime == value) return;
                _currentTime = value;
                N();
                N(nameof(Time));
                N(nameof(Date));
            }
        }

        public string Time => CurrentTime.ToShortTimeString();
        public string Date => $"{DateTimeFormat.GetDayName(CurrentTime.DayOfWeek)}, {CurrentTime.Day} {DateTimeFormat.GetMonthName(CurrentTime.Month)}";

        public ICommand OpenConfigWindowCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand ExportConfigCommand { get; }
        public ICommand ImportConfigCommand { get; }

        public MainViewModel()
        {
            _callback = HookCallback; // needed to avoid being GC'd
            _hookID = Utils.SetHook(_callback);

            App.Current.Exit += OnExit;

            try
            {
                var settings = Settings.Load();
                _root = settings.Root ?? new FolderViewModel() { IsRoot = true };
                StartWithWindows = settings.StartWithWindows;
                HotKey = settings.HotKey;
                TriggerMode = settings.TriggerMode;
                //Theme = settings.Theme;
                CloseAfterClick = settings.CloseAfterClick;
            }
            catch (Exception)
            {
                _root = new FolderViewModel() { IsRoot = true };
                HotKey = new HotKey(Keys.OemBackslash, ModifierKeys.Windows);
            }

            KeyboardHook.Instance.RegisterCallback(HotKey, OnHotKeyPressed);
            KeyboardHook.Instance.Enable();

            OpenConfigWindowCommand = new RelayCommand(OpenConfigWindow);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            ImportConfigCommand = new RelayCommand(ImportConfig);
            ExportConfigCommand = new RelayCommand(ExportConfig);

            ShortcutViewModel.Launched += OnShortcutLaunched;
            ShortcutViewModel.Hovered += OnShortcutHovered;
            ShortcutViewModel.Unhovered += OnShortcutUnhovered;

            _clockUpdateTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(0.5) };
            _clockUpdateTimer.Tick += OnClockUpdateTick;
            _clockUpdateTimer.Start();

        }
        void OnShortcutHovered(ShortcutViewModel obj)
        {
            //obj.TempRunAsAdmin = Keyboard.IsKeyDown(Key.LeftShift);
            _hoveredShortcut = obj;
        }

        void OnShortcutUnhovered(ShortcutViewModel obj)
        {
            //obj.TempRunAsAdmin = false;
            _hoveredShortcut = null;
        }


        internal async Task InitialShow()
        {
            IsVisible = true;
            await Task.Delay(1000);
            IsVisible = false;
        }


        void OnShortcutLaunched(ShortcutViewModel sender)
        {
            if (CloseAfterClick && TriggerMode == TriggerMode.Toggle)
            {
                IsVisible = false;
            }
        }

        void OnExit(object sender, ExitEventArgs e)
        {
            User32.UnhookWindowsHookEx(_hookID);
        }
        void OnHotKeyPressed()
        {
            if (_hotkeyLocked) return;
            _hotkeyLocked = true;
            IsVisible = IsVisible && TriggerMode == TriggerMode.Toggle
                ? false
                : true;
        }


        void OnClockUpdateTick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }

        void SaveConfig()
        {
            var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            var path = Process.GetCurrentProcess().MainModule?.FileName;

            if (StartWithWindows)
            {
                rk?.SetValue(nameof(PieLauncher), path!);
            }
            else
            {
                rk?.DeleteValue(nameof(PieLauncher), false);
            }

            new Settings
            {
                Root = Root,
                StartWithWindows = StartWithWindows,
                HotKey = HotKey,
                TriggerMode = TriggerMode,
                //Theme = Theme,
                CloseAfterClick = CloseAfterClick,
            }.Save();

            _configWindow?.Close();
        }

        void ImportConfig()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.ShowDialog();
                if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
                var path = ofd.FileName;

                FolderRegistry?.Clear();
                Root = Settings.LoadFrom(path).Root!;
                Root.IsRoot = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        void ExportConfig()
        {
            var file = JsonConvert.SerializeObject(Root, Settings.DefaultJsonSettings);
            File.WriteAllText(Settings.ConfigFilePath, file);
        }
        void OpenConfigWindow()
        {
            if (_configWindow != null) return;
            ForceVisible = true;
            Topmost = false;
            _configWindow = new ConfigWindow { DataContext = this };
            _configWindow.Closed += (o, e) =>
            {
                _configWindow = null;
                ForceVisible = false;
                //IsVisible = false;
                Topmost = true;
            };
            _configWindow.Show();
        }

        IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var nextCallback = User32.CallNextHookEx(_hookID, nCode, wParam, lParam);
            if (ForceVisible) return nextCallback;
            if (nCode < 0) return nextCallback;

            var key = (Keys)(Marshal.ReadInt32(lParam));
            var msg = (User32.WindowsMessages)wParam;

            //if (IsVisible && key == Keys.LShiftKey && _hoveredShortcut != null)
            //{
            //    if (msg == User32.WindowsMessages.WM_KEYUP)
            //    {
            //        _hoveredShortcut.TempRunAsAdmin = false;
            //    }
            //    else if (msg == User32.WindowsMessages.WM_KEYDOWN)
            //    {
            //        _hoveredShortcut.TempRunAsAdmin = true;
            //    }
            //}

            if (key == HotKey.Key /*Keys.OemBackslash*/)
            {
                if (msg == User32.WindowsMessages.WM_KEYDOWN &&
                    (HotKey.ModifierList.Count == 0 || HotKey.ModifierList.Any(mk => Keyboard.IsKeyDown(mk)))
                    )
                {
                    _keyDown = true;
                }
                else if (msg == User32.WindowsMessages.WM_KEYUP && _keyDown)
                {
                    _keyDown = false;
                    if (TriggerMode == TriggerMode.Hold)
                        IsVisible = false;
                    _hotkeyLocked = false;
                }
            }
            return nextCallback;
        }
    }
}
