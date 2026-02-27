namespace L4D2AddonInstaller.Services
{
    public enum InstallStage
    {
        Preparing,
        DownloadingTools,
        DownloadingPackage,
        Extracting,
        Completed
    }

    public sealed class InstallProgressInfo
    {
        public InstallStage Stage { get; set; }
        public string StatusMessage { get; set; }
        public string CurrentFileName { get; set; }
        public int Percent { get; set; }
        public long CurrentBytesDownloaded { get; set; }
        public long CurrentBytesTotal { get; set; }
        public decimal SpeedBytesPerSecond { get; set; }
        public bool IsCompleted { get; set; }
    }
}
