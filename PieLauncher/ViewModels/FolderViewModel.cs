using Microsoft.Win32;
using Newtonsoft.Json;
using Nostrum.WPF;
using Nostrum.WPF.Factories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace PieLauncher
{
    public class FolderViewModel : PieItemBase
    {

        string _name = "";
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

        public ObservableCollection<IPieItem> Apps { get; }
        [JsonIgnore]
        public ICollectionView AppsPreview { get; }

        public bool IsRoot { get; set; }

        string _iconPath = "";
        public string IconPath
        {
            get => _iconPath;
            set
            {
                if (_iconPath == value) return;
                _iconPath = value;
                N();
                N(nameof(IsIconValid));
            }
        }

        public bool IsIconValid => File.Exists(_iconPath);

        [JsonIgnore]
        public ICommand AddShortcutCommand { get; }
        [JsonIgnore]
        public ICommand AddFolderCommand { get; }
        [JsonIgnore]
        public ICommand AddSeparatorCommand { get; }
        [JsonIgnore]
        public ICommand BrowseIconCommand { get; }


        public FolderViewModel()
        {
            Apps = new();
            AddFolderCommand = new RelayCommand(AddFolder);
            AddSeparatorCommand = new RelayCommand(AddSeparator);
            AddShortcutCommand = new RelayCommand(AddShortcut);
            BrowseIconCommand = new RelayCommand(BrowseIcon);

            AppsPreview = CollectionViewFactory.CreateCollectionView(Apps, a => a is ShortcutViewModel);

            MainViewModel.FolderRegistry.Add(this);
        }

        public FolderViewModel(IList<IPieItem> apps) : this()
        {
            foreach (var app in apps)
            {
                Apps.Add(app);
            }
        }

        void AddShortcut()
        {
            Apps.Add(new ShortcutViewModel());
        }

        void AddFolder()
        {
            Apps.Add(new FolderViewModel());
        }

        void AddSeparator()
        {
            Apps.Add(new SeparatorViewModel());
        }

        void BrowseIcon()
        {
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (string.IsNullOrWhiteSpace(ofd.FileName)) return;
            IconPath = ofd.FileName;
        }

        public override string ToString()
        {
            return Name;
        }

        public override IPieItem Clone()
        {
            var ret = new FolderViewModel
            {
                Name = Name,
                IsRoot = IsRoot
            };

            foreach (var app in Apps)
            {
                ret.Apps.Add(app.Clone());
            }

            return ret;
        }
    }
}