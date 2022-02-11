using Microsoft.Win32;
using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Media.Control;

namespace PieLauncher
{
    public class MainViewModel : ObservableObject
    {
        public static readonly List<FolderViewModel> FolderRegistry = new();
        public static readonly JsonSerializerSettings DefaultJsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };

        static readonly string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), @".\config.json");
        static readonly DateTimeFormatInfo DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        readonly DispatcherTimer _clockUpdateTimer;

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

        public MediaInfoViewModel MediaInfo { get; set; } = new();

        public string Time => DateTime.Now.ToShortTimeString();
        public string Date => $"{DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}, {DateTime.Now.Day} {DateTimeFormat.GetMonthName(DateTime.Now.Month)}";

        public ICommand OpenConfigWindowCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand LoadConfigCommand { get; }

        public MainViewModel()
        {
            try
            {
                var configFileData = File.ReadAllText(ConfigFilePath);
                _root = JsonConvert.DeserializeObject<FolderViewModel>(configFileData, DefaultJsonSettings)!;
            }
            catch (Exception)
            {
                _root = new FolderViewModel();
            }
            Root.IsRoot = true;

            OpenConfigWindowCommand = new RelayCommand(OpenConfigWindow);
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfig);

            _clockUpdateTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(0.5) };
            _clockUpdateTimer.Tick += OnClockUpdateTick;
            _clockUpdateTimer.Start();
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
            new ConfigWindow { DataContext = this }.ShowDialog();
        }
    }
}
