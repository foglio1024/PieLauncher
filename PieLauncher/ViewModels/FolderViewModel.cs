using Nostrum.WPF;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PieLauncher
{
    public class FolderViewModel : ObservableObject, IPieItem
    {
        string _name = "";
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

        public ObservableCollection<IPieItem> Apps { get; }

        public FolderViewModel(IList<IPieItem> apps)
        {
            Apps = new ();

            foreach (var app in apps)
            {
                Apps.Add (app);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}