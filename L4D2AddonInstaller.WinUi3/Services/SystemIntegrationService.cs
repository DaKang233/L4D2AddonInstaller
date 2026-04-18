using Microsoft.Win32;
using System.Diagnostics;

namespace L4D2AddonInstaller.WinUi3.Services;

public sealed class SystemIntegrationService
{
    public string? GetSteamPathFromRegistry()
    {
        using var keyCu = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
        var steamPath = keyCu?.GetValue("SteamPath")?.ToString();
        if (string.IsNullOrWhiteSpace(steamPath))
        {
            using var keyLm = Registry.LocalMachine.OpenSubKey(@"Software\Valve\Steam");
            steamPath = keyLm?.GetValue("SteamPath")?.ToString();
        }

        return string.IsNullOrWhiteSpace(steamPath) ? null : steamPath.Replace("\\\\", "\\");
    }

    public bool IsProcessRunning(string processName) => Process.GetProcessesByName(processName).Any();

    public void StartExecutable(string exePath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true
        });
    }

    public void StartUri(string uri)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = uri,
            UseShellExecute = true
        });
    }

    public void OpenFolder(string folder)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = folder,
            UseShellExecute = true
        });
    }
}
