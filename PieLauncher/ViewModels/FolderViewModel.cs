using Nostrum.WPF;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PieLauncher
{
    public class FolderViewModel : ObservableObject, IPieItem
    {
        public ObservableCollection<IPieItem> Apps { get; }
        public string Name { get; set; } = "New folder";

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