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
        public static event Action<ShortcutViewModel>? Launched;

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
                N(nameof(IsAssembly));
                N(nameof(IsFolder));
                N(nameof(IsVbScript));
            }
        }

        string _args = "";
        public string Args
        {
            get => _args;
            set
            {
                if (_args == value) return;
                _args = value;
                N();
            }
        }

        string _workingDir = "";
        public string WorkingDir
        {
            get => _workingDir;
            set
            {
                if (_workingDir == value) return;
                _workingDir = value;
                N();
            }
        }

        bool _runAsAdmin;
        public bool RunAsAdmin
        {
            get => _runAsAdmin;
            set
            {
                if (_runAsAdmin == value) return;
                _runAsAdmin = value;
                N();
            }
        }

        [JsonIgnore]
        public bool IsAssembly => Uri.EndsWith(".exe") || Uri.EndsWith(".dll");
        [JsonIgnore]
        public bool IsFolder => Directory.Exists(Uri);
        [JsonIgnore]
        public bool IsVbScript => Uri.EndsWith(".vbs");

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
        [JsonIgnore]
        public ICommand BrowseWorkingDirCommand { get; }

        public ShortcutViewModel()
        {
            Name = "New shortcut";

            LaunchCommand = new RelayCommand(Launch);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            BrowseFolderCommand = new RelayCommand(BrowseFolder);
            BrowseIconCommand = new RelayCommand(BrowseIcon);
            BrowseWorkingDirCommand = new RelayCommand(BrowseWorkingDir);
        }

        void Launch()
        {
            try
            {
                var wd = Directory.Exists(WorkingDir)
                    ? WorkingDir
                    : Directory.Exists(Path.GetDirectoryName(Uri))
                        ? Path.GetDirectoryName(Uri)
                        : string.Empty;

                if (IsVbScript)
                {
                    var startInfo = new ProcessStartInfo("cscript", $"\"{Uri}\" " + Args)
                    {
                        WorkingDirectory = wd,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    if (RunAsAdmin) startInfo.Verb = "runas";
                    Process.Start(startInfo);
                }
                else
                {
                    var startInfo = new ProcessStartInfo(Uri, Args) { UseShellExecute = true };
                    if (IsAssembly && !string.IsNullOrWhiteSpace(wd))
                    {
                        startInfo.WorkingDirectory = string.IsNullOrWhiteSpace(WorkingDir)
                            ? Path.GetDirectoryName(Uri)
                            : WorkingDir;
                        if (RunAsAdmin) startInfo.Verb = "runas";
                    }
                    Process.Start(startInfo);
                }

                Launched?.Invoke(this);
            }
            catch (System.ComponentModel.Win32Exception w32ex)
            {
                if (w32ex.NativeErrorCode == 0x000004C7) return;
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
            if (string.IsNullOrWhiteSpace(ofd.SelectedPath)) return;
            SetUri(ofd.SelectedPath);
        }

        void BrowseWorkingDir()
        {
            var ofd = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (ofd.ShowDialog() != true) return;
            if (string.IsNullOrWhiteSpace(ofd.SelectedPath)) return;
            WorkingDir = ofd.SelectedPath;
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
