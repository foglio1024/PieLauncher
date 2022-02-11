using Microsoft.Win32;
using Newtonsoft.Json;
using Nostrum.WinAPI;
using Nostrum.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public static readonly JsonSerializerSettings DefaultJsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };

        static readonly string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), @".\config.json");
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
                FolderRegistry?.Clear();
                _root = value;
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

        public MediaInfoViewModel MediaInfo { get; set; } = new();

        public string Time => DateTime.Now.ToShortTimeString();
        public string Date => $"{DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}, {DateTime.Now.Day} {DateTimeFormat.GetMonthName(DateTime.Now.Month)}";

        public ICommand OpenConfigWindowCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand LoadConfigCommand { get; }

        public MainViewModel()
        {
            _callback = HookCallback; // needed to avoid being GC'd
            _hookID = Utils.SetHook(_callback);

            KeyboardHook.Instance.RegisterCallback(new HotKey(Keys.OemBackslash, ModifierKeys.Windows), OnHotKeyPressed);
            KeyboardHook.Instance.Enable();

            App.Current.Exit += OnExit;


            try
            {
                var configFileData = File.ReadAllText(ConfigFilePath);
                _root = JsonConvert.DeserializeObject<FolderViewModel>(configFileData, DefaultJsonSettings)!;
                _root.IsRoot = true;
            }
            catch (Exception)
            {
                _root = new FolderViewModel() { IsRoot = true };
            }

            OpenConfigWindowCommand = new RelayCommand(OpenConfigWindow);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfig);

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
            var file = JsonConvert.SerializeObject(Root, DefaultJsonSettings);
            File.WriteAllText(ConfigFilePath, file);
        }

        void LoadConfig()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.ShowDialog();
                if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
                var path = ofd.FileName;

                var configFileData = File.ReadAllText(path);
                Root = JsonConvert.DeserializeObject<FolderViewModel>(configFileData, DefaultJsonSettings)!;
                Root.IsRoot = true;

                File.Copy(Path.GetFullPath(path), ConfigFilePath, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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

            if (key == Keys.OemBackslash)
            {
                if (msg == User32.WindowsMessages.WM_KEYDOWN && Keyboard.IsKeyDown(Key.LWin))
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
