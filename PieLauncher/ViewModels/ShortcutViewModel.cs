﻿using Microsoft.Win32;
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
    public class ShortcutViewModel : PieItemBase
    {
        string _name = "";
        string _uri = "";
        string _iconPath = "";

        public override string Name
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
        [JsonIgnore]
        public ICommand BrowseUriCommand { get; }
        [JsonIgnore]
        public ICommand BrowseIconCommand { get; }


        string IconNameFromName => $"./{Name.ToLower().Replace(" ", "_")}.png";
        bool IsVbScript => Uri.EndsWith("vbs");

        public ShortcutViewModel()
        {
            LaunchCommand = new RelayCommand(Launch);
            BrowseUriCommand = new RelayCommand(BrowseUri);
            BrowseIconCommand = new RelayCommand(BrowseIcon);
        }

        void Launch()
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


        void BrowseUri()
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
            Uri = ofd.FileName;
        }


        void BrowseIcon()
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
            IconPath = ofd.FileName;
        }

        public override IPieItem Clone()
        {
            return new ShortcutViewModel
            {
                Name = Name,
                Uri = Uri,
                IconPath = IconPath
            };
        }
    }
}
