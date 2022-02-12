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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace PieLauncher
{
    public class MainViewModel : ObservableObject
    {
        public static readonly List<FolderViewModel> FolderRegistry = new();

        static readonly DateTimeFormatInfo DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        readonly DispatcherTimer _clockUpdateTimer;
        IntPtr _hookID = IntPtr.Zero;
        readonly User32.LowLevelKeyboardProc _callback;
        bool _keyDown = false;

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

        public string Time => DateTime.Now.ToShortTimeString();
        public string Date => $"{DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}, {DateTime.Now.Day} {DateTimeFormat.GetMonthName(DateTime.Now.Month)}";

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
                Root = settings.Root ?? new FolderViewModel() { IsRoot = true };
                StartWithWindows = settings.StartWithWindows;
                HotKey = settings.HotKey;
            }
            catch (Exception)
            {
                Root = new FolderViewModel() { IsRoot = true };
                HotKey = new HotKey(Keys.OemBackslash, ModifierKeys.Windows);
            }

            KeyboardHook.Instance.RegisterCallback(HotKey, OnHotKeyPressed);
            KeyboardHook.Instance.Enable();


            OpenConfigWindowCommand = new RelayCommand(OpenConfigWindow);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            ImportConfigCommand = new RelayCommand(ImportConfig);
            ExportConfigCommand = new RelayCommand(ExportConfig);

            _clockUpdateTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(0.5) };
            _clockUpdateTimer.Tick += OnClockUpdateTick;
            _clockUpdateTimer.Start();

        }


        void OnExit(object sender, ExitEventArgs e)
        {
            User32.UnhookWindowsHookEx(_hookID);
        }

        void OnHotKeyPressed()
        {
            IsVisible = true;
        }


        void OnClockUpdateTick(object? sender, EventArgs e)
        {
            N(nameof(Time));
            N(nameof(Date));
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
                HotKey = HotKey
            }.Save();
        }

        void ImportConfig()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.ShowDialog();
                if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
                var path = ofd.FileName;

                var configFileData = File.ReadAllText(path);
                FolderRegistry?.Clear();
                Root = JsonConvert.DeserializeObject<FolderViewModel>(configFileData, Settings.DefaultJsonSettings)!;
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
            ForceVisible = true;
            Topmost = false;
            new ConfigWindow { DataContext = this }.ShowDialog();
            ForceVisible = false;
            IsVisible = false;
            Topmost = true;
        }

        IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var nextCallback = User32.CallNextHookEx(_hookID, nCode, wParam, lParam);
            if (ForceVisible) return nextCallback;
            if (nCode < 0) return nextCallback;

            var key = (Keys)(Marshal.ReadInt32(lParam));
            var msg = (User32.WindowsMessages)wParam;

            if (key == HotKey.Key /*Keys.OemBackslash*/)
            {
                if (msg == User32.WindowsMessages.WM_KEYDOWN &&
                    (HotKey.ModifierList.Count== 0 || HotKey.ModifierList.Any(mk =>Keyboard.IsKeyDown(mk)))
                    )
                {
                    _keyDown = true;
                }
                else if (msg == User32.WindowsMessages.WM_KEYUP && _keyDown)
                {
                    _keyDown = false;
                    IsVisible = false;
                }
            }
            return nextCallback;
        }
    }
}
