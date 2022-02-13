using Microsoft.Win32;
using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PieLauncher
{
    //https://stackoverflow.com/questions/11607133/global-mouse-event-handler
    public class ShortcutViewModel : PieItemBase
    {
        string _uri = "";
        public string Uri
        {
            get => _uri;
            set
            {
                if (_uri == value) return;
                _uri = value;
                InvalidateImageCache();
                N();
            }
        }

        [JsonIgnore]
        bool IsAssembly => Uri.EndsWith(".exe") || Uri.EndsWith(".dll");
        [JsonIgnore]
        bool IsFolder => Directory.Exists(Uri);
        [JsonIgnore]
        bool IsVbScript => Uri.EndsWith("vbs");

        [JsonIgnore]
        public override ImageSource? ImageSource
        {
            get
            {
                if (_imageSourceCache != null) return _imageSourceCache;
                if (IsAssembly && string.IsNullOrEmpty(IconPath))
                {
                    IconPath = _uri + "|0";
                }
                _imageSourceCache = ImageSourceFromAssemblyOrFile();
                return _imageSourceCache;
            }
        }

        [JsonIgnore]
        public ICommand LaunchCommand { get; }
        [JsonIgnore]
        public ICommand BrowseFileCommand { get; }
        [JsonIgnore]
        public ICommand BrowseIconCommand { get; }
        [JsonIgnore]
        public ICommand BrowseFolderCommand { get; }


        public ShortcutViewModel()
        {
            Name = "New shortcut";

            LaunchCommand = new RelayCommand(Launch);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            BrowseFolderCommand = new RelayCommand(BrowseFolder);
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

        void SetUri(string uri)
        {
            Uri = uri;


            if (string.IsNullOrWhiteSpace(IconPath))
            {
                // new shortcut
                if (IsAssembly)
                {
                    // set first icon in the exe
                    SetIconFromAssembly(Uri, 0);
                }
                else
                {
                    if (IsFolder)
                    {
                        // is folder
                        SetIconFromAssembly(@"%WINDIR%\system32\shell32.dll", 3);
                    }
                    // check if is folder/local-file/url-to-something-else
                    // check https://stackoverflow.com/questions/2701263/get-the-icon-for-a-given-extension for other files
                }
            }
            else
            {
                // existing shortcut
                if (IsIconFromAssembly)
                {
                    if (IsAssembly)
                    {
                        SetIconFromAssembly(Uri, 0);
                    }
                    else
                    {
                        // do nothing if it's a folder
                        // ---
                        // todo: check on extension
                        // check https://stackoverflow.com/questions/2701263/get-the-icon-for-a-given-extension for other files
                    }
                }
            }
        }
        void BrowseFile()
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
            SetUri(ofd.FileName);
        }

        void BrowseFolder()
        {
            var ofd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (ofd.ShowDialog() != true) return;
            if(string.IsNullOrWhiteSpace(ofd.SelectedPath)) return;
            SetUri(ofd.SelectedPath);
        }

        void SetIconFromAssembly(string path, int index)
        {
            IconPath = $"{Environment.ExpandEnvironmentVariables(path)}|{index}";
        }

        void BrowseIcon()
        {
            IconPath = Utils.BrowseIcon();
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
