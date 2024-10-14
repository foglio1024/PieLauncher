using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.Input;

namespace PieLauncher.Core.ViewModels;

[JsonDerivedType(typeof(SeparatorViewModel), "SEPARATOR")]
[JsonDerivedType(typeof(ShortcutViewModel), "SHORTCUT")]
[JsonDerivedType(typeof(FolderViewModel), "FOLDER")]
public interface IPieItem
{
    string Name { get; set; }

    [JsonIgnore]
    IRelayCommand DeleteCommand { get; }

    [JsonIgnore]
    IRelayCommand CopyCommand { get; }

    [JsonIgnore]
    IRelayCommand CutCommand { get; }

    [JsonIgnore]
    IRelayCommand PasteCommand { get; }

    [JsonIgnore]
    IRelayCommand MoveToRootCommand { get; }


    IPieItem Clone();
}