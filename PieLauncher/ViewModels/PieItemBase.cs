using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PieLauncher
{
    public abstract class PieItemBase : ObservableObject, IPieItem
    {
        public abstract string Name { get; set; }

        [JsonIgnore]
        public ICommand DeleteCommand { get; }

        [JsonIgnore]
        public ICommand CopyCommand { get; }

        [JsonIgnore]
        public ICommand CutCommand { get; }

        [JsonIgnore]
        public ICommand PasteCommand { get; }

        [JsonIgnore]
        public ICommand MoveToRootCommand { get; }

        public PieItemBase()
        {
            Name = "";
            DeleteCommand = new RelayCommand(Delete);
            CopyCommand = new RelayCommand(Copy);
            CutCommand = new RelayCommand(Cut);
            PasteCommand = new RelayCommand(Paste);
            MoveToRootCommand = new RelayCommand(MoveToRoot);
        }

        void Copy()
        {
            _clipboard = this.Clone();
            //Clipboard.SetDataObject(this);//(JsonConvert.SerializeObject(this, MainViewModel.DefaultJsonSettings));
        }

        public abstract IPieItem Clone();

        void Cut()
        {
            Copy();
            Delete();
        }

        void Paste()
        {
            if (this is not FolderViewModel folder || _clipboard == null) return;
            folder.Apps.Insert(folder.Apps.Count, _clipboard); //(IPieItem)Clipboard.GetDataObject().GetData(typeof(IPieItem))); //JsonConvert.DeserializeObject<PieItemBase>(Clipboard.GetText(), MainViewModel.DefaultJsonSettings));
        }

        void MoveToRoot()
        {
            Cut();
            var rootList = MainViewModel.FolderRegistry.Single(x => x.IsRoot).Apps;
            rootList.Insert(rootList.Count, this);
        }

        void Delete()
        {
            try
            {
                var folder = MainViewModel.FolderRegistry.Single(x => x.Apps.Contains(this));
                folder.Apps.Remove(this);
                if(this is FolderViewModel fvm)
                {
                    MainViewModel.FolderRegistry.Remove(fvm);
                }
            }
            catch (Exception rx)
            {
                Debug.WriteLine($"Failed to delete item {rx}");
            }
        }

        static IPieItem? _clipboard = null;
    }
}