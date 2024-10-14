using System;

namespace Windows.Win32;

[Flags]
public enum WINDOW_STYLES
{
    WS_SYSMENU = 0x00080000,
    WS_DISABLED = 0x08000000,
    WS_EX_TOOLWINDOW = 0x00000080,
}