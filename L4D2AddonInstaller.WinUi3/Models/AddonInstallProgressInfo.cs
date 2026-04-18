using System.Collections.Generic;

namespace L4D2AddonInstaller.WinUi3.Models;

public enum InstallStage
{
    Preparing,
    Downloading,
    Completed
}

public sealed class AddonInstallProgressInfo
{
    public InstallStage Stage { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public int Percent { get; set; }
    public string ServerDisplay { get; set; } = string.Empty;
    public string ConnectCommand { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public bool ContainsArchive { get; set; }
    public IReadOnlyList<string> DownloadedArchivePaths { get; set; } = [];
    public bool IsCompleted { get; set; }
}
