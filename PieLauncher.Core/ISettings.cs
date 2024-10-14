using PieLauncher.Core.ViewModels;

namespace PieLauncher.Core;

public interface ISettings
{
    bool StartWithWindows { get; set; }
    bool CloseAfterClick { get; set; }
    TriggerMode TriggerMode { get; set; }
    Theme Theme { get; set; }
    public HotKey HotKey { get; set; }
    FolderViewModel? Root{ get; set; }
}
