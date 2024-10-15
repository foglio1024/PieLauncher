using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

using Nostrum.WinAPI;

using PieLauncher.Core;
using PieLauncher.Core.Services;
using PieLauncher.Core.ViewModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PieLauncher.Avalonia.Services;

public class StorageService : IStorageService
{
    private readonly IClassicDesktopStyleApplicationLifetime _app;

    private IReadOnlyList<FilePickerFileType>? _iconFileTypes =
    [
        new FilePickerFileType("Icons or assemblies")
        {
            Patterns = [".jpg", ".png", ".gif", ".webp", ".bmp", ".ico", ".exe", ".dll"]
        }
    ];

    private IStorageProvider _storageProvider
    {
        get
        {
            try
            {
                var requestingWindow = _app.Windows.Single(x => x.IsActive);
                return requestingWindow.StorageProvider;
            }
            catch (Exception)
            {
                // ignore
            }
            return _app.MainWindow!.StorageProvider;
        }
    }

    public StorageService(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _app = desktop;
    }

    public async Task SaveSettingsAsync(ISettings settings)
    {
        var file = JsonSerializer.Serialize(settings);

        if (!Directory.Exists(Path.GetDirectoryName(IStorageService.ConfigFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(IStorageService.ConfigFilePath));
        }

        await File.WriteAllTextAsync(IStorageService.ConfigFilePath, file);
    }

    public ISettings LoadSettings()
    {
        return JsonSerializer.Deserialize<Settings>(File.ReadAllText(IStorageService.ConfigFilePath));
    }

    public async Task<string> BrowseFileAsync()
    {
        var file = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            FileTypeFilter = [FilePickerFileTypes.All],
            Title = "Select a file to be launched",
            AllowMultiple = false
        });

        return file.FirstOrDefault()?.Path.AbsolutePath ?? "";
    }

    public async Task<string> BrowseFolderAsync()
    {
        var file = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Select a folder to be opened",
            AllowMultiple = false
        });

        return file.FirstOrDefault()?.Path.AbsolutePath ?? "";
    }

    public async Task<string> BrowseIcon(string name = "")
    {
        var file = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            FileTypeFilter = _iconFileTypes,
            Title = "Select an icon" + (string.IsNullOrEmpty(name) ? "" : $" {name}"),
            AllowMultiple = false
        });

        if (file.Count != 1) return "";
        var path = file.Single().Path.AbsolutePath;
        var ext = Path.GetExtension(path);
        if (ext is ".exe" or ".dll")
        {
            var sb = new StringBuilder(path, 256);
            if (!Shell32.SHPickIconDialog(IntPtr.Zero, sb, 256, out var idx)) return "";
            return (sb + "|" + idx);
        }

        return path;
    }

    public Task ExportDataAsync(FolderViewModel root)
    {
        throw new NotImplementedException();
    }

    public Task<FolderViewModel?> ImportDataAsync()
    {
        throw new NotImplementedException();
    }
}