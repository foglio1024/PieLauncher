using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace PieLauncher
{
    public abstract class PieItemBase : ObservableObject, IPieItem
    {
        public abstract string Name { get; set; }

        [JsonIgnore]
        public ICommand DeleteCommand { get; }

        public PieItemBase()
        {
            Name = "";
            DeleteCommand = new RelayCommand(Delete);
        }

        void Delete()
        {
            try
            {
                var folder = MainViewModel.FolderRegistry.Single(x => x.Apps.Contains(this));
                folder.Apps.Remove(this);
            }
            catch (Exception rx)
            {
                Debug.WriteLine($"Failed to delete item {rx}");
            }
        }
    }
}