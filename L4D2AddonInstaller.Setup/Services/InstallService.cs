using L4D2AddonInstaller.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static L4D2AddonInstaller.Parsers.SteamLibraryVdfParser;

namespace L4D2AddonInstaller.Services
{
    public sealed class InstallService : IInstallService
    {
        public async Task InstallAsync(string installRootPath, VersionDetails versionDetails, IProgress<InstallProgressInfo> progress, CancellationToken cancellationToken)
        {
            if (versionDetails == null) throw new ArgumentNullException(nameof(versionDetails));

            var installPath = Path.Combine(installRootPath, InstallerForm.InstallerAppName);
            var downloadArchivePath = versionDetails.RelativePath;
            var archiveFileName = Path.GetFileName(downloadArchivePath);

            progress?.Report(new InstallProgressInfo { Stage = InstallStage.Preparing, StatusMessage = "准备安装...", Percent = 0 });

            await SevenZipHelper.Download7ZipExeToDirectory(cancellationToken, installPath, new Progress<int>(value =>
            {
                progress?.Report(new InstallProgressInfo
                {
                    Stage = InstallStage.DownloadingTools,
                    StatusMessage = value >= 100 ? "7-Zip 下载完成。" : "正在下载 7-Zip 组件...",
                    Percent = Math.Max(0, Math.Min(100, value))
                });
            }));

            var downloadList = HttpHelper.GetDownloadList(versionDetails.Protocol, versionDetails.WebServer, versionDetails.WebPort, versionDetails.Prefix,
                new List<string> { downloadArchivePath }, installPath);

            await HttpHelper.DownloadListItemsWithByteProgressAsync(downloadList.Items, cancellationToken,
                new Progress<HttpHelper.DownloadByteProgressInfo>(info =>
                {
                    var totalBytesSafe = info.TotalBytes > 0 ? info.TotalBytes : info.CurrentFileTotalBytes;
                    var downloadedBytesSafe = info.TotalBytes > 0 ? info.TotalBytesDownloaded : info.CurrentFileBytesDownloaded;
                    var percent = totalBytesSafe > 0 ? (int)Math.Min(100, (downloadedBytesSafe * 100M / totalBytesSafe)) : 0;

                    progress?.Report(new InstallProgressInfo
                    {
                        Stage = InstallStage.DownloadingPackage,
                        StatusMessage = info.IsCompleted ? "文件下载完成。" : $"正在下载 {info.CurrentFileName} ({HttpHelper.GetBytesUnitString(info.CurrentFileBytesDownloaded)}/{HttpHelper.GetBytesUnitString(info.CurrentFileTotalBytes)})",
                        CurrentFileName = info.CurrentFileName,
                        Percent = percent,
                        CurrentBytesDownloaded = info.CurrentFileBytesDownloaded,
                        CurrentBytesTotal = info.CurrentFileTotalBytes,
                        SpeedBytesPerSecond = Convert.ToDecimal(Math.Max(info.CurrentFileSpeedBytesPerSec, 0d)),
                        IsCompleted = info.IsCompleted
                    });
                }));

            var archiveFullPath = Path.Combine(installPath, archiveFileName);
            await SevenZipHelper.ExtractAsync(
                archiveFullPath,
                installPath,
                Path.Combine(installPath, "tools", "7z.exe"),
                new Progress<int>(value =>
                {
                    progress?.Report(new InstallProgressInfo
                    {
                        Stage = InstallStage.Extracting,
                        StatusMessage = $"正在解压文件... ({value}%)",
                        Percent = Math.Max(0, Math.Min(100, value)),
                        CurrentFileName = archiveFileName
                    });
                }),
                null,
                SevenZipHelper.OverwriteMode.OverwriteAll,
                cancellationToken);

            progress?.Report(new InstallProgressInfo { Stage = InstallStage.Completed, StatusMessage = "安装完成。", Percent = 100, IsCompleted = true });
        }
    }
}
