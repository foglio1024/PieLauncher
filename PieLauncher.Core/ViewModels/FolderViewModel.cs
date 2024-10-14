#pragma warning disable CS0657 // Not a valid attribute location for this declaration

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PieLauncher.Core.Messages;

namespace PieLauncher.Core.ViewModels;

public partial class FolderViewModel : PieItemBase, IDragDropHandler
{

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<IPieItem> Apps { get; }
    public IList DropDestination => Unsafe.As<IList>(Apps);

    public bool IsRoot { get; set; }

    public override string IconPath
    {
        get => _iconPath;
        set
        {
            if (_iconPath == value) return;
            base.IconPath = value;
            OnPropertyChanged(nameof(IsIconValid));
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

    public FolderViewModel()
    {
        Apps = [];
        base.Name = "New group";

        Apps.CollectionChanged += OnAppsCollectionChanged;

        WeakReferenceMessenger.Default.Send(new RegisterFolderMessage(this));
        //MainViewModel.FolderRegistry.Add(this);
    }

    private void OnAppsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add) return;

        foreach (var newItem in e.NewItems?.Cast<PieItemBase>() ?? [])
        {
            newItem.Parent = this;
        }
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void AddShortcut()
    {
        Apps.Add(new ShortcutViewModel() { Parent = this });
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void AddFolder()
    {
        Apps.Add(new FolderViewModel() { Parent = this });
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void AddSeparator()
    {
        Apps.Add(new SeparatorViewModel() { Parent = this });
    }

    [RelayCommand]
    [property: JsonIgnore]
    private async Task BrowseIcon()
    {
        IconPath = await WeakReferenceMessenger.Default.Send(new RequestBrowseIconMessage());
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

    public void StartDrag(IDragDropItem item)
    {
    }

    public bool CanDrop(IDragDropItem item, IEnumerable? destination)
    {
        switch  (item)
        {
            case FolderViewModel { IsRoot: true }:
            case FolderViewModel when !this.IsRoot:
                return false;
            default:
                return ReferenceEquals(destination, Apps);
        }
    }

    public void Drop(IDragDropItem item, IEnumerable? destination, int index)
    {
        if (!ReferenceEquals(destination, Apps) || item is not IPieItem pieItem) return;

        if (index >= 0 && index < Apps.Count)
        {
            Apps.Insert(index, pieItem);
        }
        else
        {
            Apps.Add(pieItem);
        }
    }

}