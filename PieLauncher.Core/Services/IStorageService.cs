using System.IO;
using PieLauncher.Core.ViewModels;

namespace PieLauncher.Core.Services;

public interface IStorageService
{
    public static string ConfigFilePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PieLauncher" , @".\config.json");
    //public static string ConfigFilePath { get; } = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, @".\config.json");

    Task SaveSettingsAsync(ISettings settings);
    Task<string> BrowseFileAsync();
    Task<string> BrowseFolderAsync();
    Task<string> BrowseIcon(string name = "");
    ISettings LoadSettings();
    Task ExportDataAsync(FolderViewModel root);
    Task<FolderViewModel?> ImportDataAsync();
}