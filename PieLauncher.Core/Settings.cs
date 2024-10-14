using PieLauncher.Core.ViewModels;

namespace PieLauncher.Core;

public record struct Settings(
    bool StartWithWindows,
    bool CloseAfterClick,
    FolderViewModel? Root, 
    HotKey HotKey = new(), 
    TriggerMode TriggerMode = TriggerMode.Hold, 
    Theme Theme = Theme.Dark
    )
    : ISettings;