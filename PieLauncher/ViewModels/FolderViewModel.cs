using Newtonsoft.Json;
using Nostrum.WPF;
using Nostrum.WPF.Factories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace PieLauncher
{
    public class FolderViewModel : PieItemBase
    {


        public ObservableCollection<IPieItem> Apps { get; }

        public bool IsRoot { get; set; }

        public override string IconPath
        {
            get => _iconPath;
            set
            {
                if (_iconPath == value) return;
                base.IconPath = value;
                N(nameof(IsIconValid));
            }
        }

        public bool IsIconValid => !string.IsNullOrWhiteSpace(_iconPath);

        [JsonIgnore]
        public override ImageSource? ImageSource
        {
            get
            {
                if (_imageSourceCache != null) return _imageSourceCache;
                _imageSourceCache = ImageSourceFromAssemblyOrFile();
                return _imageSourceCache;
            }
        }

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
            Name = "New group";
            AddFolderCommand = new RelayCommand(AddFolder);
            AddSeparatorCommand = new RelayCommand(AddSeparator);
            AddShortcutCommand = new RelayCommand(AddShortcut);
            BrowseIconCommand = new RelayCommand(BrowseIcon);


            MainViewModel.FolderRegistry.Add(this);
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
            IconPath = Utils.BrowseIcon();
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