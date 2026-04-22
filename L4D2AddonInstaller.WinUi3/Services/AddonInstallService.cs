using L4D2AddonInstaller.WinUi3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace L4D2AddonInstaller.WinUi3.Services;

public sealed class AddonInstallService
{
    private const string DownloadListUrl = "https://furina.dakang233.com:8443/www/l4d2/download.txt";

    public async Task<AddonInstallProgressInfo> ResolveServerInfoAsync(string code, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var config = await GetConfigByCodeAsync(code, token);
        var host = GetConfigValue(config, "gameServerHost");
        var port = GetConfigValue(config, "gameServerPort");
        return BuildServerInfo(host, port);
    }

    public async Task<AddonInstallProgressInfo> DownloadAndInstallAsync(string code, string gamePath, IProgress<AddonInstallProgressInfo>? progress, CancellationToken token)
    {
        var config = await GetConfigByCodeAsync(code, token);
        var host = GetConfigValue(config, "gameServerHost");
        var port = GetConfigValue(config, "gameServerPort");
        var webServer = GetConfigValue(config, "webServer");
        var webPort = GetConfigValue(config, "port");
        var protocol = GetConfigValue(config, "protocol", "https");
        var prefix = GetConfigValue(config, "prefix");

        var addonPaths = SteamLibraryVdfParserModern.GetAddonPathsFromConfig(config);
        if (!addonPaths.Any())
            throw new InvalidOperationException("该代号无需要下载的附加组件");

        var addonsInstallPath = Path.Combine(gamePath.Trim(), "left4dead2", "addons");
        var archiveDownloadPath = Path.Combine(gamePath.Trim(), "l4d2InstallToolDownloads");
        var downloadArchivePaths = new List<string>();

        var downloadList = HttpHelperModern.GetDownloadList(protocol, webServer, webPort, prefix, addonPaths, addonsInstallPath);
        foreach (var item in downloadList.Items)
        {
            var ext = Path.GetExtension(item.FileName);
            if (ext.Equals(".zip", StringComparison.OrdinalIgnoreCase) || ext.Equals(".7z", StringComparison.OrdinalIgnoreCase))
            {
                Directory.CreateDirectory(archiveDownloadPath);
                item.SavePath = Path.Combine(archiveDownloadPath, item.FileName);
                downloadArchivePaths.Add(item.SavePath);
            }
        }

        await HttpHelperModern.DownloadListItemsWithByteProgressAsync(downloadList.Items, token,
            new Progress<HttpHelperModern.DownloadByteProgressInfo>(info =>
            {
                var server = BuildServerInfo(host, port);
                progress?.Report(new AddonInstallProgressInfo
                {
                    Stage = info.IsCompleted ? InstallStage.Completed : InstallStage.Downloading,
                    StatusMessage = info.IsCompleted ? "所有附加组件下载完成。" : $"正在下载 {info.CurrentFileName}",
                    Percent = CalculatePercent(info),
                    Host = server.Host,
                    Port = server.Port,
                    ServerDisplay = server.ServerDisplay,
                    ConnectCommand = server.ConnectCommand,
                    ContainsArchive = downloadArchivePaths.Count > 0,
                    DownloadedArchivePaths = downloadArchivePaths,
                    IsCompleted = info.IsCompleted
                });
            }));

        var done = BuildServerInfo(host, port);
        done.Stage = InstallStage.Completed;
        done.StatusMessage = downloadArchivePaths.Count > 0 ? "所有附加组件已下载；检测到压缩包(尚未解压)。" : "所有附加组件下载并安装完成！";
        done.Percent = 100;
        done.ContainsArchive = downloadArchivePaths.Count > 0;
        done.DownloadedArchivePaths = downloadArchivePaths;
        done.IsCompleted = true;
        return done;
    }

    private static int CalculatePercent(HttpHelperModern.DownloadByteProgressInfo info)
    {
        if (info.IsCompleted) return 100;
        var total = info.TotalBytes > 0 ? info.TotalBytes : info.CurrentFileTotalBytes;
        var done = info.TotalBytes > 0 ? info.TotalBytesDownloaded : info.CurrentFileBytesDownloaded;
        if (total <= 0) return 0;
        return (int)Math.Min(99, done * 100M / total);
    }

    private static async Task<Dictionary<string, object>> GetConfigByCodeAsync(string code, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("下载代号不能为空", nameof(code));

        var text = await HttpHelperModern.GetRemoteTextAsync(DownloadListUrl);
        token.ThrowIfCancellationRequested();

        var config = SteamLibraryVdfParserModern.GetAddonConfigByCode(text, code.Trim());
        if (config is null)
            throw new InvalidOperationException($"未找到代号「{code}」的下载配置");

        return config;
    }

    private static string GetConfigValue(Dictionary<string, object> config, string key, string fallback = "")
        => config.TryGetValue(key, out var value) ? value?.ToString() ?? fallback : fallback;

    private static AddonInstallProgressInfo BuildServerInfo(string host, string port)
    {
        var isDefaultPort = string.IsNullOrWhiteSpace(port) || port == "27015";
        return new AddonInstallProgressInfo
        {
            Host = host,
            Port = port,
            ServerDisplay = isDefaultPort ? host : $"{host}:{port}",
            ConnectCommand = isDefaultPort ? $"connect {host}" : $"connect {host}:{port}"
        };
    }
}
