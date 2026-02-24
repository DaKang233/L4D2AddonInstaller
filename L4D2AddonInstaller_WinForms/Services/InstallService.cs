using L4D2AddonInstaller.Helper;
using L4D2AddonInstaller.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace L4D2AddonInstaller.Services
{
    public sealed class InstallService : IInstallService
    {
        private const string DownloadListUrl = "https://furina.dakang233.com:8443/www/l4d2/download.txt";

        public async Task<InstallProgressInfo> ResolveServerInfoAsync(string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var config = await GetConfigByCodeAsync(code, cancellationToken);
            var host = GetConfigValue(config, "gameServerHost");
            var port = GetConfigValue(config, "gameServerPort");
            return BuildServerInfo(host, port);
        }

        public async Task<InstallProgressInfo> DownloadAndInstallAsync(string code, string gamePath, IProgress<InstallProgressInfo> progress, CancellationToken cancellationToken)
        {
            var config = await GetConfigByCodeAsync(code, cancellationToken);
            var host = GetConfigValue(config, "gameServerHost");
            var port = GetConfigValue(config, "gameServerPort");
            var webServer = GetConfigValue(config, "webServer");
            var webPort = GetConfigValue(config, "port");
            var protocol = GetConfigValue(config, "protocol", "https");
            var prefix = GetConfigValue(config, "prefix");

            var addonPaths = SteamLibraryVdfParser.GetAddonPathsFromConfig(config);
            if (!addonPaths.Any())
                throw new InvalidOperationException("该代号无需要下载的附加组件");

            var addonsInstallPath = Path.Combine(gamePath.Trim(), "left4dead2", "addons");
            var archiveDownloadPath = Path.Combine(gamePath.Trim(), "l4d2InstallToolDownloads");
            var downloadArchivePaths = new List<string>();

            var downloadList = HttpHelper.GetDownloadList(protocol, webServer, webPort, prefix, addonPaths, addonsInstallPath);
            foreach (var item in downloadList.Items)
            {
                var ext = Path.GetExtension(item.FileName);
                if (string.Equals(ext, ".zip", StringComparison.OrdinalIgnoreCase) || string.Equals(ext, ".7z", StringComparison.OrdinalIgnoreCase))
                {
                    Directory.CreateDirectory(archiveDownloadPath);
                    item.SavePath = Path.Combine(archiveDownloadPath, item.FileName);
                    downloadArchivePaths.Add(item.SavePath);
                }
            }

            await HttpHelper.DownloadListItemsWithByteProgressAsync(downloadList.Items, cancellationToken,
                new Progress<HttpHelper.DownloadByteProgressInfo>(info =>
                {
                    var percent = CalculatePercent(info);
                    var serverInfo = BuildServerInfo(host, port);
                    progress?.Report(new InstallProgressInfo
                    {
                        Stage = info.IsCompleted ? InstallStage.Completed : InstallStage.Downloading,
                        StatusMessage = info.IsCompleted ? "所有附加组件下载完成。" : $"正在下载 {info.CurrentFileName}",
                        Percent = percent,
                        Host = serverInfo.Host,
                        Port = serverInfo.Port,
                        ServerDisplay = serverInfo.ServerDisplay,
                        ConnectCommand = serverInfo.ConnectCommand,
                        ContainsArchive = downloadArchivePaths.Count > 0,
                        DownloadedArchivePaths = downloadArchivePaths.AsReadOnly(),
                        IsCompleted = info.IsCompleted
                    });
                }));

            var done = BuildServerInfo(host, port);
            done.Stage = InstallStage.Completed;
            done.StatusMessage = downloadArchivePaths.Count > 0 ? "所有附加组件已下载；检测到压缩包(尚未解压)。" : "所有附加组件下载并安装完成！";
            done.Percent = 100;
            done.ContainsArchive = downloadArchivePaths.Count > 0;
            done.DownloadedArchivePaths = downloadArchivePaths.AsReadOnly();
            done.IsCompleted = true;
            return done;
        }

        private static int CalculatePercent(HttpHelper.DownloadByteProgressInfo info)
        {
            if (info.IsCompleted)
                return 100;

            var totalBytesSafe = info.TotalBytes > 0 ? info.TotalBytes : info.CurrentFileTotalBytes;
            var downloadedBytesSafe = info.TotalBytes > 0 ? info.TotalBytesDownloaded : info.CurrentFileBytesDownloaded;

            if (totalBytesSafe > 0)
                return (int)Math.Min(99, downloadedBytesSafe * 100M / totalBytesSafe);

            if (info.TotalFiles > 0)
            {
                var basePercent = (int)Math.Min(99, info.CompletedFiles * 100M / info.TotalFiles);
                if (info.CurrentFileBytesDownloaded > 0)
                    return Math.Max(basePercent, 1);
                return basePercent;
            }

            return info.CurrentFileBytesDownloaded > 0 ? 1 : 0;
        }

        private static async Task<Dictionary<string, object>> GetConfigByCodeAsync(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("下载代号不能为空", nameof(code));

            var downloadListContent = await HttpHelper.GetRemoteTextAsync(DownloadListUrl);
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(downloadListContent))
                throw new InvalidOperationException("下载配置为空");

            var config = SteamLibraryVdfParser.GetAddonConfigByCode(downloadListContent, code.Trim());
            if (config == null)
                throw new InvalidOperationException($"未找到代号「{code}」的下载配置");

            return config;
        }

        private static string GetConfigValue(Dictionary<string, object> config, string key, string fallback = "")
        {
            return config.TryGetValue(key, out var value) ? value?.ToString() ?? fallback : fallback;
        }

        private static InstallProgressInfo BuildServerInfo(string host, string port)
        {
            var isDefaultPort = string.Equals(port, "27015", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(port);
            return new InstallProgressInfo
            {
                Host = host,
                Port = port,
                ServerDisplay = isDefaultPort ? host : $"{host}:{port}",
                ConnectCommand = isDefaultPort ? $"connect {host}" : $"connect {host}:{port}"
            };
        }
    }
}
