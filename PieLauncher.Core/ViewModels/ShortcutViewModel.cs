#pragma warning disable CS0657 // Not a valid attribute location for this declaration

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PieLauncher.Core.Messages;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace PieLauncher.Core.ViewModels;

//https://stackoverflow.com/questions/11607133/global-mouse-event-handler
public partial class ShortcutViewModel : PieItemBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAssembly))]
    [NotifyPropertyChangedFor(nameof(IsFolder))]
    [NotifyPropertyChangedFor(nameof(IsVbScript))]
    [NotifyPropertyChangedFor(nameof(IsBatScript))]
    private string _uri = "";

    [ObservableProperty]
    private string _args = "";

    [ObservableProperty]
    private string _workingDir = "";

    private bool _canRunAsAdmin;

    public bool CanRunAsAdmin
    {
        get => _canRunAsAdmin || TempRunAsAdmin;
        set
        {
            if (_canRunAsAdmin == value) return;
            _canRunAsAdmin = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private bool _isHovered;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRunAsAdmin))]
    private bool _tempRunAsAdmin;

    [JsonIgnore]
    public bool IsAssembly => Uri.EndsWith(".exe") || Uri.EndsWith(".dll");

    [JsonIgnore]
    public bool IsFolder => Directory.Exists(Uri);

    [JsonIgnore]
    public bool IsVbScript => Uri.EndsWith(".vbs");

    [JsonIgnore]
    public bool IsBatScript => Uri.EndsWith(".bat");

    [JsonIgnore]
    public override ImageSource? ImageSource
    {
        get
        {
            if (_imageSourceCache != null) return _imageSourceCache;
            if (IsAssembly && string.IsNullOrEmpty(IconPath))
            {
                IconPath = Uri + "|0";
            }
            _imageSourceCache = ImageSourceFromAssemblyOrFile();
            return _imageSourceCache;
        }
    }

    public ShortcutViewModel()
    {
        base.Name = "New shortcut";
    }

    [RelayCommand]
    [property: JsonIgnore]
    private void Launch(bool forceAdmin = false)
    {
        var verb = forceAdmin
            ? "runas"
            : "";

        var containingFolder = Path.GetDirectoryName((string?)Uri);
        var wd = Directory.Exists(WorkingDir)
            ? WorkingDir
            : Directory.Exists(containingFolder)
                ? containingFolder
                : string.Empty;

        ProcessStartInfo startInfo = new()
        {
            Verb = verb
        };

        if (IsVbScript)
        {
            startInfo.FileName = "cscript";
            startInfo.Arguments = $"\"{Uri}\" {Args}";
            startInfo.WorkingDirectory = wd;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        }
        else if (IsBatScript)
        {
            startInfo.FileName = "cmd";
            startInfo.Arguments = $"/c \"{Uri}\" {Args}";
            startInfo.WorkingDirectory = wd;
        }
        else
        {
            startInfo.FileName = Uri;
            startInfo.Arguments = Args;
            startInfo.UseShellExecute = true;

            if (IsAssembly && !string.IsNullOrWhiteSpace(wd))
            {
                startInfo.WorkingDirectory = wd;
            }
        }

        Task.Run(() =>
        {
            try
            {
                Process.Start(startInfo);
            }
            catch (Win32Exception /*w32Ex*/)
            {
                //if (w32Ex.NativeErrorCode == 0x000004C7) return;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Pie launcher");
            }
        });

        WeakReferenceMessenger.Default.Send(new ShortcutLaunchedMessage(this));

    }

    [RelayCommand]
    [property: JsonIgnore]
    private async Task BrowseFile()
    {
        var fn = await WeakReferenceMessenger.Default.Send<RequestBrowseFileMessage>();
        if (string.IsNullOrWhiteSpace(fn)) return;
        SetUri(fn);
    }

    [RelayCommand]
    [property: JsonIgnore]
    private async Task BrowseFolder()
    {
        var fn = await WeakReferenceMessenger.Default.Send(new RequestBrowseFolderMessage());

        SetUri(fn);
    }

    [RelayCommand]
    [property: JsonIgnore]
    private async Task BrowseWorkingDir()
    {
        WorkingDir = await WeakReferenceMessenger.Default.Send(new RequestBrowseFolderMessage());
    }

    [RelayCommand]
    [property: JsonIgnore]
    private async Task BrowseIcon()
    {
        IconPath = await WeakReferenceMessenger.Default.Send(new RequestBrowseIconMessage());
    }

    private void SetUri(string uri)
    {
        Uri = uri;

        if (string.IsNullOrWhiteSpace(IconPath))
        {
            // new shortcut
            if (IsAssembly)
            {
                // set first icon in the exe
                SetIconFromAssembly(Uri, 0);
            }
            else if (IsFolder)
            {
                // is folder
                SetIconFromAssembly(@"%WINDIR%\system32\shell32.dll", 3);
            }
            // check if is folder/local-file/url-to-something-else
            // check https://stackoverflow.com/questions/2701263/get-the-icon-for-a-given-extension for other files
        }
        // existing shortcut
        else if (IsIconFromAssembly)
        {
            if (IsAssembly)
            {
                SetIconFromAssembly(Uri, 0);
            }
            else
            {
                // do nothing if it's a folder
                // ---
                // todo: check on extension
                // check https://stackoverflow.com/questions/2701263/get-the-icon-for-a-given-extension for other files
            }
        }
    }

    public override string ToString()
    {
        return Name;
    }

    private void SetIconFromAssembly(string path, int index)
    {
        IconPath = $"{Environment.ExpandEnvironmentVariables(path)}|{index}";
    }

    public override IPieItem Clone()
    {
        return new ShortcutViewModel
        {
            Name = Name,
            Uri = Uri,
            IconPath = IconPath
        };
    }

    partial void OnIsHoveredChanged(bool value)
    {
        WeakReferenceMessenger.Default.Send(new ShortcutHoveredMessage(this, value));
    }

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnUriChanged(string value)
    {
        InvalidateImageCache();
    }
}