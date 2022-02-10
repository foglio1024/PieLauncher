using Newtonsoft.Json;
using Nostrum.WPF;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ICommand AddShortcutCommand { get; }
        [JsonIgnore]
        public ICommand AddFolderCommand { get; }
        [JsonIgnore]
        public ICommand AddSeparatorCommand { get; }

        public FolderViewModel()
        {
            Apps = new();
            AddFolderCommand = new RelayCommand(AddFolder);
            AddSeparatorCommand = new RelayCommand(AddSeparator);
            AddShortcutCommand = new RelayCommand(AddShortcut);

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

        public override string ToString()
        {
            return Name;
        }
    }
}