using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Media.Control;

namespace PieLauncher
{
    public class MainViewModel : ObservableObject
    {
        public static List<FolderViewModel> FolderRegistry { get; } = new List<FolderViewModel>();

        static readonly string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @".\config.json");
        public static readonly JsonSerializerSettings DefaultJsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };
        static readonly DateTimeFormatInfo DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        readonly DispatcherTimer _clockUpdateTimer;

        public FolderViewModel Root { get; }
        public MediaInfoViewModel MediaInfo { get; set; } = new();

        public string Time => DateTime.Now.ToShortTimeString();
        public string Date => $"{DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}, {DateTime.Now.Day} {DateTimeFormat.GetMonthName(DateTime.Now.Month)}";

        public ICommand OpenConfigWindowCommand { get; }
        public ICommand SaveConfigCommand { get; }

        public MainViewModel()
        {
            var configFileData = File.ReadAllText(ConfigFilePath);
            Root = JsonConvert.DeserializeObject<FolderViewModel>(configFileData, DefaultJsonSettings)!;
            Root.IsRoot = true;

            OpenConfigWindowCommand = new RelayCommand(OpenConfigWindow);
            SaveConfigCommand = new RelayCommand(SaveConfig);

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

        void OpenConfigWindow()
        {
            new ConfigWindow { DataContext = this }.ShowDialog();
        }
    }
}
