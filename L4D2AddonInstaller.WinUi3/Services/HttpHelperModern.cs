namespace L4D2AddonInstaller.WinUi3.Services;

public static class HttpHelperModern
{
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromMinutes(5) };

    public sealed class DownloadItem
    {
        public Uri FileUri { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string SavePath { get; set; } = string.Empty;
    }

    public sealed class DownloadListResult
    {
        public List<DownloadItem> Items { get; set; } = [];
    }

    public sealed class DownloadByteProgressInfo
    {
        public int TotalFiles { get; init; }
        public int CompletedFiles { get; init; }
        public string CurrentFileName { get; init; } = string.Empty;
        public long CurrentFileBytesDownloaded { get; init; }
        public long CurrentFileTotalBytes { get; init; }
        public long TotalBytesDownloaded { get; init; }
        public long TotalBytes { get; init; }
        public bool IsCompleted { get; init; }
    }

    public static async Task<string> GetRemoteTextAsync(string url)
    {
        using var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static DownloadListResult GetDownloadList(string protocol, string webServer, string webPort, string prefix, List<string> relativePaths, string savePath)
    {
        var result = new DownloadListResult();
        var baseUri = new Uri($"{protocol}://{webServer}:{webPort}/");
        var normalizedPrefix = (prefix ?? string.Empty).Trim().Trim('/');

        foreach (var path in relativePaths)
        {
            if (string.IsNullOrWhiteSpace(path))
                continue;

            var p = path.Trim();
            Uri uri;
            if (Uri.TryCreate(p, UriKind.Absolute, out var absolute))
            {
                uri = absolute;
            }
            else
            {
                var rel = p.TrimStart('/');
                var combined = string.IsNullOrEmpty(normalizedPrefix) ? rel : $"{normalizedPrefix}/{rel}";
                uri = new Uri(baseUri, combined);
            }

            var fileName = Path.GetFileName(uri.LocalPath);
            if (string.IsNullOrWhiteSpace(fileName))
                continue;

            result.Items.Add(new DownloadItem
            {
                FileUri = uri,
                FileName = fileName,
                SavePath = Path.Combine(savePath, fileName)
            });
        }

        return result;
    }

    public static async Task DownloadListItemsWithByteProgressAsync(List<DownloadItem> items, CancellationToken token, IProgress<DownloadByteProgressInfo>? progress)
    {
        var totalBytes = 0L;
        foreach (var item in items)
            totalBytes += await GetContentLengthAsync(item.FileUri, token);

        var downloadedTotal = 0L;
        var completed = 0;

        foreach (var item in items)
        {
            token.ThrowIfCancellationRequested();
            Directory.CreateDirectory(Path.GetDirectoryName(item.SavePath)!);

            var currentDownloaded = 0L;
            var currentTotal = await GetContentLengthAsync(item.FileUri, token);
            using var response = await Client.GetAsync(item.FileUri, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();
            await using var input = await response.Content.ReadAsStreamAsync(token);
            await using var output = File.Create(item.SavePath);

            var buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = await input.ReadAsync(buffer, token)) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                currentDownloaded += bytesRead;
                downloadedTotal += bytesRead;
                progress?.Report(new DownloadByteProgressInfo
                {
                    TotalFiles = items.Count,
                    CompletedFiles = completed,
                    CurrentFileName = item.FileName,
                    CurrentFileBytesDownloaded = currentDownloaded,
                    CurrentFileTotalBytes = currentTotal,
                    TotalBytesDownloaded = downloadedTotal,
                    TotalBytes = totalBytes,
                    IsCompleted = false
                });
            }

            completed++;
            progress?.Report(new DownloadByteProgressInfo
            {
                TotalFiles = items.Count,
                CompletedFiles = completed,
                CurrentFileName = item.FileName,
                CurrentFileBytesDownloaded = currentTotal,
                CurrentFileTotalBytes = currentTotal,
                TotalBytesDownloaded = downloadedTotal,
                TotalBytes = totalBytes,
                IsCompleted = completed == items.Count
            });
        }
    }

    private static async Task<long> GetContentLengthAsync(Uri uri, CancellationToken token)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Head, uri);
            using var resp = await Client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, token);
            if (resp.IsSuccessStatusCode && resp.Content.Headers.ContentLength.HasValue)
                return resp.Content.Headers.ContentLength.Value;
        }
        catch
        {
        }

        return 0;
    }
}
