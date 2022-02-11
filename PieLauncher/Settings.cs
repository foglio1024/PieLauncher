using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.IO;

namespace PieLauncher
{
    public class Settings : ObservableObject
    {
        public static readonly JsonSerializerSettings DefaultJsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };
        public static readonly string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, @".\config.json");

        public FolderViewModel? Root { get; set; }
        public bool StartWithWindows { get; set; }

        public static Settings Load()
        {
            var configFileData = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<Settings>(configFileData, DefaultJsonSettings)!;
        }

        public void Save()
        {
            var file = JsonConvert.SerializeObject(this, DefaultJsonSettings);
            File.WriteAllText(ConfigFilePath, file);
        }

    }
}
