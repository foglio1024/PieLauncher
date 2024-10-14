using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Nostrum.WinAPI;
using PieLauncher.Core.Messages;
using PieLauncher.Core.Services;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace PieLauncher.Core.ViewModels;

using Microsoft.Win32;

public partial class MainViewModel :
    ObservableObject,
    IRecipient<ShortcutLaunchedMessage>,
    IRecipient<AppReadyMessage>,
    IRecipient<AppExitingMessage>,
    IRecipient<RegisterFolderMessage>,
    IRecipient<RequestBrowseFileMessage>,
    IRecipient<RequestBrowseFolderMessage>,
    IRecipient<RequestBrowseIconMessage>,
    IRecipient<MoveItemToRootMessage>,
    IRecipient<DeleteItemMessage>,
    IDragDropHandler
{
    private readonly List<FolderViewModel> _folderRegistry = [];

    private static readonly DateTimeFormatInfo DateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;

    private readonly DispatcherTimer _clockUpdateTimer;
    private readonly IStorageService _storage;
    private readonly IWindowManager _windowManager;
    private readonly ITrayIcon _tray;
    private readonly IntPtr _hookId;
    private bool _hotkeyLocked;
    private bool _keyDown;
    //private ConfigWindow? _configWindow;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly User32.LowLevelKeyboardProc? _callback;

    [ObservableProperty] private FolderViewModel _root;
    [ObservableProperty] private HotKey _hotKey;
    [ObservableProperty] private TriggerMode _triggerMode;
    [ObservableProperty] private Theme _theme;
    [ObservableProperty] private bool _closeAfterClick;
    [ObservableProperty] private bool _topmost = true;
    [ObservableProperty] private bool _isVisible;
    [ObservableProperty] private bool _forceVisible;
    [ObservableProperty] private bool _startWithWindows;
    [ObservableProperty] private IPieItem? _selectedItem;

    [NotifyPropertyChangedFor(nameof(Time))]
    [NotifyPropertyChangedFor(nameof(Date))]
    [ObservableProperty] private DateTime _currentTime = DateTime.Now;

    public string Time => CurrentTime.ToShortTimeString();
    public string Date => $"{DateTimeFormat.GetDayName(CurrentTime.DayOfWeek)}, {CurrentTime.Day} {DateTimeFormat.GetMonthName(CurrentTime.Month)}";

    public MediaInfoViewModel MediaInfo { get; }

    public MainViewModel(ITrayIcon trayIcon, IStorageService storage, IWindowManager windowManager, bool design)
    {
        WeakReferenceMessenger.Default.RegisterAll(this);

        _windowManager = windowManager;

        _tray = trayIcon;
        _tray.Clicked += OnTrayClicked;

        _storage = storage;

        if (!design)
        {
            _callback = HookCallback; // needed to avoid being GC'd
            _hookId = Utils.SetHook(_callback);
        }

        try
        {
            var settings = _storage.LoadSettings();// Settings.Load();
            _root = settings.Root ?? new FolderViewModel() { IsRoot = true };
            StartWithWindows = settings.StartWithWindows;
            HotKey = settings.HotKey;
            TriggerMode = settings.TriggerMode;
            //Theme = settings.Theme;
            CloseAfterClick = settings.CloseAfterClick;
        }
        catch (Exception)
        {
            _root = new FolderViewModel() { IsRoot = true };
            HotKey = new HotKey(Keys.OemBackslash, ModifierKeys.Windows);
        }

        KeyboardHook.Instance.RegisterCallback(HotKey, OnHotKeyPressed);
        KeyboardHook.Instance.Enable();

        _clockUpdateTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(0.5) };
        _clockUpdateTimer.Tick += OnClockUpdateTick;
        _clockUpdateTimer.Start();
    }

    [RelayCommand]
    private void SaveConfig()
    {
#if !DEBUG

        using var rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        var path = Process.GetCurrentProcess().MainModule?.FileName;

        if (StartWithWindows)
        {
            rk?.SetValue(nameof(PieLauncher), path!);
        }
        else
        {
            rk?.DeleteValue(nameof(PieLauncher), false);
        }

#endif
        _storage.SaveSettingsAsync(new Settings
        {
            Root = Root,
            StartWithWindows = StartWithWindows,
            HotKey = HotKey,
            TriggerMode = TriggerMode,
            //Theme = Theme,
            CloseAfterClick = CloseAfterClick,
        });

        _windowManager.CloseConfigWindow();
    }

    [RelayCommand]
    private async Task ImportConfig()
    {
        try
        {
            var imported = await _storage.ImportDataAsync();
            if (imported == null) return;

            _folderRegistry.Clear();

            Root = imported;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    [RelayCommand]
    private async Task ExportConfig()
    {
        await _storage.ExportDataAsync(Root);
    }

    [RelayCommand]
    private void OpenConfigWindow()
    {
        _windowManager.ShowConfigWindow(this);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        var nextCallback = User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        if (ForceVisible) return nextCallback;
        if (nCode < 0) return nextCallback;

        var key = (Keys)(Marshal.ReadInt32(lParam));
        if (key != HotKey.Key) return nextCallback;

        var msg = (User32.WindowsMessages)wParam;
        switch (msg)
        {
            case User32.WindowsMessages.WM_KEYDOWN when
                (HotKey.ModifierList.Count == 0 || HotKey.ModifierList.Any(Keyboard.IsKeyDown)):
                _keyDown = true;
                break;
            case User32.WindowsMessages.WM_KEYUP when _keyDown:
                {
                    _keyDown = false;
                    if (TriggerMode == TriggerMode.Hold)
                        IsVisible = false;

                    _hotkeyLocked = false;
                    break;
                }
        }

        return nextCallback;
    }

    partial void OnThemeChanged(Theme value)
    {
        WeakReferenceMessenger.Default.Send(new ChangeThemeMessage(value));
    }

    partial void OnHotKeyChanged(HotKey oldValue, HotKey newValue)
    {
        KeyboardHook.Instance.ChangeHotkey(oldValue, newValue);
    }

    private async Task InitialShow()
    {
        IsVisible = true;
        await Task.Delay(1000);
        IsVisible = false;
    }

    private void OnHotKeyPressed()
    {
        if (_hotkeyLocked) return;
        _hotkeyLocked = true;
        IsVisible = IsVisible && TriggerMode == TriggerMode.Toggle
            ? false
            : true;
    }

#if false
    int day = 0;
#endif

    private void OnClockUpdateTick(object? sender, EventArgs e)
    {
#if false
        day++;

        if (day >= 5)
        {
            day = 0;
            CurrentTime = CurrentTime.AddDays(1);
        }
#else
        CurrentTime = DateTime.Now;
#endif
    }

    partial void OnIsVisibleChanged(bool value)
    {
        WeakReferenceMessenger.Default.Send(new ShowHideWindowMessage(value));
    }

    private void OnTrayClicked()
    {
        if (ForceVisible) return;
        IsVisible = !IsVisible;
    }

    public void Receive(ShortcutLaunchedMessage message)
    {
        if (CloseAfterClick)
        {
            IsVisible = false;
        }
    }

    public void Receive(AppExitingMessage message)
    {
        User32.UnhookWindowsHookEx(_hookId);
    }

    public void Receive(AppReadyMessage message)
    {
        _ = InitialShow();
    }

    public void Receive(RegisterFolderMessage message)
    {
        _folderRegistry.Add(message.Folder);
    }

    public void Receive(RequestBrowseFileMessage message)
    {
        message.Reply(_storage.BrowseFileAsync());
    }

    public void Receive(RequestBrowseFolderMessage message)
    {
        message.Reply(_storage.BrowseFolderAsync());
    }

    public void Receive(RequestBrowseIconMessage message)
    {
        message.Reply(_storage.BrowseIcon());
    }

    public void Receive(MoveItemToRootMessage message)
    {
        var rootList = _folderRegistry.Single(x => x.IsRoot).Apps;
        rootList.Insert(rootList.Count, message.Item);
    }

    public void Receive(DeleteItemMessage message)
    {
        try
        {
            var item = message.Item;

            var folder = _folderRegistry.Single(x => x.Apps.Contains(item));
            folder.Apps.Remove(item);

            if (item is FolderViewModel fvm)
            {
                _folderRegistry.Remove(fvm);
            }
        }
        catch (Exception rx)
        {
            Debug.WriteLine($"Failed to delete item {rx}");
        }
    }


    public void StartDrag(IDragDropItem item)
    {
    }

    public bool CanDrop(IDragDropItem item, IEnumerable? destination)
    {
        return Root.CanDrop(item, destination);
    }

    public void Drop(IDragDropItem item, IEnumerable? destination, int index)
    {
        Root.Drop(item, destination, index);
    }

    public IList DropDestination => Root.Apps;
}