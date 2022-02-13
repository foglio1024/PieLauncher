using Newtonsoft.Json;
using Nostrum.WPF;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PieLauncher
{
    public abstract class PieItemBase : ObservableObject, IPieItem
    {
        protected ImageSource? _imageSourceCache;
        protected string _iconPath = "";

        protected string _name = "";
        public virtual string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                N();
            }
        }

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
        public virtual string IconPath
        {
            get => _iconPath;
            set
            {
                if (_iconPath == value) return;
                _iconPath = value;
                InvalidateImageCache();
                N();
            }
        }
        [JsonIgnore]
        protected bool IsIconFromAssembly
        {
            get
            {
                var spl = IconPath.Split('|');
                if (spl.Length != 2) return false;
                if ((spl[0].EndsWith(".exe") || spl[0].EndsWith(".dll")) && int.TryParse(spl[1], out _)) return true;
                return false;
            }
        }

        [JsonIgnore]
        public int AssemblyIconIndex => IsIconFromAssembly ? int.Parse(IconPath.Split('|')[1]) : -1;
        [JsonIgnore]
        public string AssemblyIconPath => IsIconFromAssembly ? IconPath.Split("|")[0] : "";
        [JsonIgnore]
        public virtual ImageSource? ImageSource { get; }

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
            folder.Apps.Insert(folder.Apps.Count, _clipboard); 
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
                if (this is FolderViewModel fvm)
                {
                    MainViewModel.FolderRegistry.Remove(fvm);
                }
            }
            catch (Exception rx)
            {
                Debug.WriteLine($"Failed to delete item {rx}");
            }
        }

        protected ImageSource? ImageSourceFromAssemblyOrFile()
        {
            try
            {
                return IsIconFromAssembly
                    ? Utils.ExtractIconFromAssembly(AssemblyIconPath, AssemblyIconIndex)
                    : new BitmapImage(new Uri(_iconPath, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                 return null;
            }
        }

        protected void InvalidateImageCache()
        {
            _imageSourceCache = null;
            N(nameof(ImageSource));
        }

        static IPieItem? _clipboard = null;
    }
}