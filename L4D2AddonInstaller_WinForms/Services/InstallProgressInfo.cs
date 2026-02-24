using System;
using System.Collections.Generic;

namespace L4D2AddonInstaller.Services
{
    public enum InstallStage
    {
        Preparing,
        Downloading,
        Completed
    }

    public sealed class InstallProgressInfo
    {
        public InstallStage Stage { get; set; }
        public string StatusMessage { get; set; }
        public int Percent { get; set; }
        public string ServerDisplay { get; set; }
        public string ConnectCommand { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public bool ContainsArchive { get; set; }
        public IReadOnlyList<string> DownloadedArchivePaths { get; set; }
        public bool IsCompleted { get; set; }
    }
}
