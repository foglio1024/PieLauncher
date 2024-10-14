using PieLauncher.Core;
using PieLauncher.Core.Services;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;
using Nostrum.WinAPI;
using System.Text;
using System;
using PieLauncher.Core.ViewModels;

namespace PieLauncher;

internal class StorageService : IStorageService
{
    public async Task SaveSettingsAsync(ISettings settings)
    {
        var file = JsonSerializer.Serialize(settings);
        await File.WriteAllTextAsync(IStorageService.ConfigFilePath, file);
    }

    public Task<string> BrowseFileAsync()
    {
        var dialog = new OpenFileDialog();
        return Task.FromResult(dialog.ShowDialog() != true ? "" : dialog.FileName);
    }

    public Task<string> BrowseFolderAsync()
    {
        var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
        return Task.FromResult(dialog.ShowDialog() != true ? "" : dialog.SelectedPath);
    }

    public Task<string> BrowseIcon(string name = "")
    {
        var dialog = new OpenFileDialog
        {
            Title = $"Browse icon{(string.IsNullOrWhiteSpace(name) ? "" : $" for {name}")}",
            Filter = "Image Files(*.jpg;*.png;*.gif;*.webp;*.bmp;*.ico)|*.jpg;*.png;*.gif;*.webp;*.bmp;*.ico|Assemblies (*.exe;*.dll)|*.exe;*.dll|All files (*.*)|*.*"
        };
        dialog.ShowDialog();
        if (string.IsNullOrWhiteSpace(dialog.FileName)) return Task.FromResult("");
        if (dialog.FileName.EndsWith(".exe") || dialog.FileName.EndsWith(".dll"))
        {
            var sb = new StringBuilder(dialog.FileName, 256);
            if (!Shell32.SHPickIconDialog(IntPtr.Zero, sb, 256, out var idx)) return Task.FromResult("");
            return Task.FromResult(sb + "|" + idx);
        }
        else
        {
            return Task.FromResult(dialog.FileName);
        }
    }

    public ISettings LoadSettings()
    {
        return JsonSerializer.Deserialize<Settings>(File.ReadAllText(IStorageService.ConfigFilePath));
    }

    public async Task ExportDataAsync(FolderViewModel root)
    {
        var dialog = new OpenFileDialog();
        dialog.ShowDialog();

        if (string.IsNullOrWhiteSpace(dialog.FileName)) return;

        await using var file = File.Create(dialog.FileName);

        await JsonSerializer.SerializeAsync(file, root);
    }

    public async Task<FolderViewModel?> ImportDataAsync()
    {
        var dialog = new OpenFileDialog();
        dialog.ShowDialog();

        if (string.IsNullOrWhiteSpace(dialog.FileName)) return null;

        await using var file = File.OpenRead(dialog.FileName);

        var ret = await JsonSerializer.DeserializeAsync<Settings>(file);

        if(ret.Root != null)
            ret.Root.IsRoot = true;

        return ret.Root;
    }
}