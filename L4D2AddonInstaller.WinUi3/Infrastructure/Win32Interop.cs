using System;
using WinRT.Interop;

namespace L4D2AddonInstaller.WinUi3.Infrastructure;

public static class Win32Interop
{
    public static IntPtr GetWindowHandle(Microsoft.UI.Xaml.Window window) => WindowNative.GetWindowHandle(window);
}
