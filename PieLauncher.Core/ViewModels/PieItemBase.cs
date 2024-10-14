#pragma warning disable CS0657 // Not a valid attribute location for this declaration

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PieLauncher.Core.Messages;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PieLauncher.Core.ViewModels;

public interface IDragDropItem;

public abstract partial class PieItemBase : ObservableObject, IPieItem, IDragDropItem
{
    private static IPieItem? _clipboard;
    protected ImageSource? _imageSourceCache;
    protected string _iconPath = "";

    private string _name = "";

    public virtual string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            OnPropertyChanged();
        }
    }
    
    public virtual string IconPath
    {
        get => _iconPath;
        set
        {
            if (_iconPath == value) return;
            _iconPath = value;
            InvalidateImageCache();
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
    public PieItemBase? Parent{ get; set; }

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
    public abstract ImageSource? ImageSource { get; }

    [RelayCommand]
    [property: JsonIgnore]
    private void Copy()
    {
        _clipboard = this.Clone();
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void Cut()
    {
        Copy();
        Delete();
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void Paste()
    {
        if (this is not FolderViewModel folder || _clipboard == null) return;
        folder.Apps.Insert(folder.Apps.Count, _clipboard);
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void MoveToRoot()
    {
        Cut();
        WeakReferenceMessenger.Default.Send(new MoveItemToRootMessage(this));
        
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void Delete()
    {
        WeakReferenceMessenger.Default.Send(new DeleteItemMessage(this));
    }

    protected ImageSource? ImageSourceFromAssemblyOrFile()
    {
        try
        {
            if (IsIconFromAssembly && !File.Exists(AssemblyIconPath)) return null;
            if (!IsIconFromAssembly && !File.Exists(_iconPath)) return null;

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
        OnPropertyChanged(nameof(ImageSource));
    }

    public abstract IPieItem Clone();
}