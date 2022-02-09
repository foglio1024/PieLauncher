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
        DispatcherTimer _clockUpdateTimer;

        public ObservableCollection<IPieItem> Apps { get; }

        public MediaInfoViewModel MediaInfo { get; set; } = new();

        public ICommand OpenSettingsCommand { get; }

        public string Time => DateTime.Now.ToShortTimeString();
        public string Date => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}, {DateTime.Now.Day} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)}";

        public MainViewModel()
        {
            Apps = JsonConvert.DeserializeObject<ObservableCollection<IPieItem>>(File.ReadAllText(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @".\config.json")
                    ),
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })!;

            OpenSettingsCommand = new RelayCommand(OpenSettings);
        
            _clockUpdateTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(0.5)};
            _clockUpdateTimer.Start();
            _clockUpdateTimer.Tick += OnClockUpdateTick;
        }

        void OnClockUpdateTick(object? sender, EventArgs e)
        {
            N(nameof(Time));
            N(nameof(Date));
        }

        void OpenSettings()
        {
            new ConfigWindow { DataContext = this }.ShowDialog();
        }
    }
}
