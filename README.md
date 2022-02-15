<img align="left" width="100" height="100" src="https://i.imgur.com/zs12ISF.png">

# PieLauncher
&nbsp;


![](https://i.imgur.com/dhM4XBv.png)

PieLauncher is an application launcher for Windows. It allows you to add shortcuts to applications, files, folders, urls.

## Requirements
This application requires .NET 6 Desktop Runtime, download [here](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.2-windows-x64-installer).

## Download and installation
Download is available in the [Releases](https://github.com/Foglio1024/PieLauncher/releases) section. Simply download **PieLauncher.exe** and run it.

## Custom icons and media controls
It's possible to set a custom icons for every shortcut. It also features a basic integration with media services to display info about currently playing music.

<p align="center">
<img width="500" src="https://i.imgur.com/4uqwF0o.png">
</p>
<p align="center">
<i>Media controls</i>
</p>

## Groups
Items can be grouped to save space.

<p align="center">
<img width="300" src="https://i.imgur.com/TxqKXrN.png">
</p>
<p align="center">
<i>Shortcut groups</i>
</p>

## Interaction and tray icon

The launcher can be displayed at any time by holding the customizable hotkey (default: <kbd>Win</kbd> + <kbd><</kbd>). An icon is always available in system tray; it can be clicked once to open settings window, it can be double clicked to close the application.

## Settings
Pressing the cogwheel button in the main window will open settings. Here's a quick overview:
- **Start with Windows**: when enabled, PieLauncher will start with your system
- **Hotkey**: click on the textbox and press the key combination you want to use to show the main window. Be careful to release the key before the modifiers (ctrl, win, alt, shift) or the hotkey won't register properly
- **Items**: here you can configure the various items
   - hover the "Add new item" button to display the various types of items you can add (shortcut, group, separator)
   - drag and drop to reorder them

![](https://i.imgur.com/X7z0WTp.png)

<p align="center">
<i>Settings window</i>
</p>

### Item types and settings
- **Shortcut**, can point to any executable, file, folder, url
   - **Path**: the path of the file to execute, folder to open or url to browse to. You can type the path or use the browse buttons on the right.
   - **Icon path**: the path of the icon to display for the shortcut. It can be an image file or an icon inside an assembly (.exe or .dll files). It will be automatically set to the first icon in the target file when "Path" is an executable. The button on the right can be used to browse to an image file or to an assembly (in this case, a new dialog to select an icon from the assembly will be displayed).
   - **Arguments**: command line args to be passed to the target application.
   - **Working dir**: directory used as working dir by the target application. If it's not specified, the folder of the executable will be used.
- **Group**, can hold shortcuts and more folders
   - **Icon path**: same as the shortcut one.
- **Separator**, used to separate items
