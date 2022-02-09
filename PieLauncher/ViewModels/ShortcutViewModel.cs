using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PieLauncher
{
    //https://stackoverflow.com/questions/11607133/global-mouse-event-handler
    public class ShortcutViewModel : ObservableObject, IPieItem
    {
        string _name = "";
        string _uri = "";
        string _iconPath = "";

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                N();
            }
        }
        public string Uri
        {
            get => _uri;
            set
            {
                if (_uri == value) return;
                _uri = value;
                N();
            }
        }
        public string IconPath
        {
            get => _iconPath; 
            set
            {
                if(_iconPath == value) return;
                if (value.EndsWith(".exe"))
                {
                    if (File.Exists(IconNameFromName))
                    {
                        _iconPath = Path.GetFullPath(IconNameFromName);
                        return;
                    }
                    // proper way of doing this: http://www.vbaccelerator.com/home/NET/Code/Libraries/Shell_Projects/SysImageList/article.html
                    var icon = Icon.ExtractAssociatedIcon(value);
                    if (icon is not null)
                    {
                        using var fs = new FileStream($"./{IconNameFromName}", FileMode.CreateNew);
                        icon.Save(fs);
                        _iconPath = fs.Name;
                    }
                }
                else
                {
                    _iconPath = value;
                }
                N();

            }
        }

        [JsonIgnore]
        public ICommand LaunchCommand { get; }

        string IconNameFromName => $"./{Name.ToLower().Replace(" ", "_")}.png";
        bool IsVbScript => Uri.EndsWith("vbs");

        public ShortcutViewModel()
        {
            LaunchCommand = new RelayCommand(Launch);
        }

        void Launch(object? obj)
        {
            try
            {
                if (IsVbScript)
                {
                    Process.Start(new ProcessStartInfo("cscript", $"\"{Uri}\"") { WorkingDirectory = Path.GetDirectoryName(Uri), WindowStyle = ProcessWindowStyle.Hidden });
                }
                else
                {
                    Process.Start(new ProcessStartInfo(Uri) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pie launcher");
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SeparatorViewModel : IPieItem
    {
        public string Name { get; set; } = "---";
        public override string ToString()
        {
            return Name;
        }
    }
}
