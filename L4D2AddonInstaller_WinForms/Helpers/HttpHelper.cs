using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L4D2AddonInstaller.Helper
{
    /// <summary>
    /// HTTPS请求工具类（处理ZeroSSL证书，获取远程文件）
    /// </summary>
    public class HttpHelper
    {
        // 全局HttpClient（避免频繁创建释放）
        private static readonly HttpClient _httpClient;

        static HttpHelper()
        {
            // 配置HttpClient：忽略证书验证（若ZeroSSL证书在客户端信任则可注释）
            /*
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    // 生产环境建议验证证书指纹，而非直接返回true
                    // 示例：return cert.Thumbprint == "你的ZeroSSL证书指纹";
                    return true;
                }
            };
            */
            _httpClient = new HttpClient(/*handler*/)
            {
                Timeout = TimeSpan.FromMinutes(5) // 下载超时时间
            };
        }

        /// <summary>
        /// 异步获取远程文本文件（如download.txt）
        /// </summary>
        public static async Task<string> GetRemoteTextAsync(string url)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode(); // 抛出HTTP错误（4xx/5xx）
                    return await response.Content.ReadAsStringAsync(); // 获取文本内容
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"网络请求失败：{ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception($"请求超时：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 异步下载文件到指定路径（带进度回调）
        /// </summary>
        /// <param name="url">文件的URL地址</param>
        /// <param name="savePath">文件的保存路径</param>
        /// <param name="progress">进度回调接口，用于报告下载进度百分比</param>
        public static async Task DownloadFileAsync(
            string url,
            string savePath,
            CancellationToken cancellationToken,
            IProgress<int> progress = null)
        {
            HttpResponseMessage response = null;
            CancellationTokenRegistration ctr = default;

            try
            {
                response = await _httpClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead
                );

                response.EnsureSuccessStatusCode();

                // 注册取消回调：一旦取消，立刻 Dispose response
                ctr = cancellationToken.Register(() =>
                {
                    try { response.Dispose(); } catch { }
                });

                var totalBytes = response.Content.Headers.ContentLength ?? 0;
                var downloadedBytes = 0L;
                var buffer = new byte[8192];

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = File.Create(savePath))
                {
                    int bytesRead;
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        // 主动检查取消
                        cancellationToken.ThrowIfCancellationRequested();

                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0 && progress != null)
                        {
                            int percent = (int)(downloadedBytes * 100.0 / totalBytes);
                            progress.Report(percent);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);

                throw; // 非常重要：向上传递“取消”
            }
            catch (ObjectDisposedException)
            {
                // HttpResponse 被 Dispose，通常意味着取消
                if (File.Exists(savePath))
                    File.Delete(savePath);

                throw new OperationCanceledException(cancellationToken);
            }
            catch (Exception ex)
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);

                throw new Exception($"下载文件失败：{ex.Message}", ex);
            }
            finally
            {
                ctr.Dispose();
                response?.Dispose();
            }
        }

        /// <summary>
        /// 异步下载文件，并实时报告已下载的字节数
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="filePath">本地保存路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <param name="progress">用于报告已下载的字节数</param>
        public static async Task DownloadFileAsyncWithByteProgress(
            string url,
            string filePath,
            CancellationToken cancellationToken,
            IProgress<long> progress = null)
        {
            CancellationTokenRegistration ctr = default;
            
            try {
                var response = await _httpClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);
                response.EnsureSuccessStatusCode();

                ctr = cancellationToken.Register(() =>
                {
                    try { response?.Dispose(); } catch { }
                });

                var buffer = new byte[8192];
                long bytesDownloaded = 0;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    int bytesRead;
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        bytesDownloaded += bytesRead;

                        // 报告已下载的字节数
                        progress?.Report(bytesDownloaded);
                    }
                }
            }
            catch (ObjectDisposedException) 
            {
                if (File.Exists(filePath)) File.Delete(filePath);
                throw new OperationCanceledException(cancellationToken);
            }
            catch (OperationCanceledException) 
            {
                if (File.Exists(filePath)) File.Delete(filePath);
                throw;
            }
            catch (Exception ex) { throw new Exception("下载文件失败：", ex); }
        }

        public class DownloadItem
        {
            public Uri FileUri { get; set; }
            public string FileName { get; set; }
            public string SavePath { get; set; }
        }

        public class DownloadListResult
        {
            public List<DownloadItem> Items { get; set; }
            public List<string> Skipped { get; set; }
        }

        /// <summary>
        /// 规范化并构造最终下载项
        /// </summary>
        /// <param name="protocol">协议</param>
        /// <param name="webServer">服务器</param>
        /// <param name="webPort">端口</param>
        /// <param name="prefix">前缀</param>
        /// <param name="downloadRelativePaths">下载路径列表</param>
        /// <param name="savePath">下载的保存路径</param>
        /// <returns>下载列表结构 (类 DownloadListResult)</returns>
        /// <exception cref="ArgumentNullException">部分参数为空</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        public static DownloadListResult GetDownloadList(
            string protocol,
            string webServer,
            string webPort,
            string prefix,
            List<string> downloadRelativePaths,
            string savePath)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(webServer))
                throw new ArgumentNullException(nameof(webServer), "Web服务器地址不能为空");
            if (string.IsNullOrWhiteSpace(savePath))
                throw new ArgumentNullException(nameof(savePath), "保存路径不能为空");
            if (downloadRelativePaths == null)
                throw new ArgumentNullException(nameof(downloadRelativePaths), "下载路径列表不能为空");

            var downloadItems = new List<DownloadItem>();
            var skipped = new List<string>();
            Uri baseUri = null;
            try
            {
                baseUri = new Uri($"{protocol}://{webServer}:{webPort}/");
            }
            catch (UriFormatException ex) { throw new ArgumentException("Base URL 格式错误：",ex); }

            var normalizedPrefix = (prefix ?? string.Empty).Trim().Trim('/');
            foreach (var path in downloadRelativePaths)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    skipped.Add(path);
                    continue;
                }
                var p = path.Trim();

                try
                {
                    Uri finalUri;
                    if (Uri.TryCreate(p, UriKind.Absolute, out finalUri))
                    {
                        // 已经是完整 URL
                        finalUri = new Uri(p);
                    }
                    else
                    {
                        var rel = p.TrimStart('/');
                        // 拼接前缀和相对路径（如果前缀为空则直接使用相对路径）
                        var combined = string.IsNullOrEmpty(normalizedPrefix) ? rel : $"{normalizedPrefix}/{rel}";
                        finalUri = new Uri(baseUri, combined);
                    }

                    var fileName = Path.GetFileName(finalUri.LocalPath);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        skipped.Add(p);
                        continue;
                    }
                    
                    var savePathSingle = Path.Combine(savePath, fileName);
                    downloadItems.Add(new DownloadItem
                    {
                        FileUri = finalUri,
                        FileName = fileName,
                        SavePath = savePathSingle
                    });
                }
                catch
                {
                    skipped.Add(p);
                }
            }
            DownloadListResult result = new DownloadListResult();
            result.Items = downloadItems;
            result.Skipped = skipped;
            return result;
        }

        public class DownloadProgressInfo
        {
            public int TotalFiles { get; }
            public int CompletedFiles { get; }

            public string CurrentFileName { get; }
            public int CurrentFilePercent { get; } // 0-100

            public bool IsCompleted { get; }

            public DownloadProgressInfo(
                int totalFiles,
                int completedFiles,
                string currentFileName,
                int currentFilePercent,
                bool isCompleted)
            {
                TotalFiles = totalFiles;
                CompletedFiles = completedFiles;
                CurrentFileName = currentFileName;
                CurrentFilePercent = currentFilePercent;
                IsCompleted = isCompleted;
            }
        }

        /// <summary>
        /// 下载指定列表里的文件，按百分比报告进度
        /// </summary>
        /// <param name="downloadItems">下载列表</param>
        /// <param name="cancellationToken">取消Token</param>
        /// <param name="progressReporter">进度报告（百分比）</param>
        /// <returns></returns>
        public static async Task DownloadListItemsAsync(
            List<DownloadItem> downloadItems,
            CancellationToken cancellationToken,
            IProgress<DownloadProgressInfo> progressReporter)
        {
            int totalFiles = downloadItems.Count;
            int downloadedFiles = 0;

            var semaphore = new SemaphoreSlim(3);
            var tasks = new List<Task>();

            foreach (var item in downloadItems)
            {
                await semaphore.WaitAsync(cancellationToken);

                tasks.Add(Task.Run(async () =>
                {
                    var uri = item.FileUri;
                    var fileName = item.FileName;
                    var savePath = item.SavePath;

                    try
                    {
                        var fileProgress = new Progress<int>(percent =>
                        {
                            progressReporter?.Report(
                                new DownloadProgressInfo(
                                    totalFiles,
                                    downloadedFiles,
                                    fileName,
                                    percent,
                                    false));
                        });

                        await HttpHelper.DownloadFileAsync(
                            uri.ToString(),
                            savePath,
                            cancellationToken,
                            fileProgress);

                        int completed = Interlocked.Increment(ref downloadedFiles);

                        progressReporter?.Report(
                            new DownloadProgressInfo(
                                totalFiles,
                                completed,
                                fileName,
                                100,
                                completed == totalFiles));
                    }
                    catch (OperationCanceledException)
                    {
                        progressReporter?.Report(
                            new DownloadProgressInfo(
                                totalFiles,
                                downloadedFiles,
                                fileName,
                                0,
                                false));
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 下载进度信息（按字节数）
        /// </summary>
        public class DownloadByteProgressInfo
        {
            public int TotalFiles { get; }
            public int CompletedFiles { get; }

            public string CurrentFileName { get; }

            public long CurrentFileBytesDownloaded { get; }
            public long CurrentFileTotalBytes { get; }

            public long TotalBytesDownloaded { get; }
            public long TotalBytes { get; }

            public double CurrentFileSpeedBytesPerSec { get; }

            public bool IsCompleted { get; }

            /// <summary>
            /// 下载进度信息（按字节数）
            /// </summary>
            /// <param name="totalFiles">要下载的总文件数</param>
            /// <param name="completedFiles">已下载的文件数</param>
            /// <param name="currentFileName">当前下载的文件名</param>
            /// <param name="currentFileBytesDownloaded">正在下载的文件的已下载字节</param>
            /// <param name="currentFileTotalBytes">正在下载的文件的总字节</param>
            /// <param name="totalBytesDownloaded">已下载的总字节</param>
            /// <param name="totalBytes">要下载的总字节</param>
            /// <param name="currentFileSpeedBytesPerSec">下载速率</param>
            /// <param name="isCompleted">是否已全部下载完成</param>
            public DownloadByteProgressInfo(
                int totalFiles,
                int completedFiles,
                string currentFileName,
                long currentFileBytesDownloaded,
                long currentFileTotalBytes,
                long totalBytesDownloaded,
                long totalBytes,
                double currentFileSpeedBytesPerSec,
                bool isCompleted)
            {
                TotalFiles = totalFiles;
                CompletedFiles = completedFiles;
                CurrentFileName = currentFileName;
                CurrentFileBytesDownloaded = currentFileBytesDownloaded;
                CurrentFileTotalBytes = currentFileTotalBytes;
                TotalBytesDownloaded = totalBytesDownloaded;
                TotalBytes = totalBytes;
                CurrentFileSpeedBytesPerSec = currentFileSpeedBytesPerSec;
                IsCompleted = isCompleted;
            }
        }

        /// <summary>
        /// 批量下载指定列表里的文件，按字节数报告进度
        /// </summary>
        /// <param name="downloadItems">表示下载列表，元素类型为DownloadItem</param>
        /// <param name="cancellationToken">供取消按钮使用</param>
        /// <param name="progressReporter">精确的进度报告</param>
        /// <returns></returns>
        public static async Task DownloadListItemsWithByteProgressAsync(
            List<DownloadItem> downloadItems,
            CancellationToken cancellationToken,
            IProgress<DownloadByteProgressInfo> progressReporter)
        {
            int totalFiles = downloadItems.Count;
            int completedFiles = 0;

            long totalBytes = 0;
            foreach (var item in downloadItems)
                totalBytes += await GetContentLengthAsync(item.FileUri.ToString());

            long totalBytesDownloaded = 0;
            var semaphore = new SemaphoreSlim(3);
            var tasks = new List<Task>();

            foreach (var item in downloadItems)
            {
                await semaphore.WaitAsync(cancellationToken);

                tasks.Add(Task.Run(async () =>
                {
                    var fileName = item.FileName;
                    var url = item.FileUri.ToString();
                    var savePath = item.SavePath;

                    long fileTotalBytes = await GetContentLengthAsync(url);
                    long lastBytes = 0;
                    long lastSampleBytes = 0;
                    const long sampleIntervalMs = 500;
                    var speedSamples = new Queue<decimal>(4);
                    var sw = Stopwatch.StartNew();

                    try
                    {
                        var fileProgress = new Progress<long>(bytesDownloaded =>
                        {
                            var deltaBytes = bytesDownloaded - lastBytes;
                            if (deltaBytes <= 0)
                                return;

                            lastBytes = bytesDownloaded;
                            var currentTotalBytesDownloaded = Interlocked.Add(ref totalBytesDownloaded, deltaBytes);

                            decimal speed = 0m;
                            var elapsedMs = sw.ElapsedMilliseconds;
                            if (elapsedMs >= sampleIntervalMs)
                            {
                                var sampleDelta = bytesDownloaded - lastSampleBytes;
                                if (sampleDelta > 0)
                                {
                                    speed = sampleDelta * 1000m / elapsedMs;
                                    if (speedSamples.Count == 4)
                                        speedSamples.Dequeue();
                                    speedSamples.Enqueue(speed);
                                }

                                lastSampleBytes = bytesDownloaded;
                                sw.Restart();
                            }

                            if (speed == 0m && speedSamples.Count > 0)
                            {
                                decimal sum = 0m;
                                foreach (var sample in speedSamples) sum += sample;
                                speed = sum / speedSamples.Count;
                            }

                            progressReporter?.Report(
                                new DownloadByteProgressInfo(
                                    totalFiles,
                                    Volatile.Read(ref completedFiles),
                                    fileName,
                                    bytesDownloaded,
                                    fileTotalBytes,
                                    currentTotalBytesDownloaded,
                                    totalBytes,
                                    (double)speed,
                                    false));
                        });

                        await DownloadFileAsyncWithByteProgress(url, savePath, cancellationToken, fileProgress);

                        if (fileTotalBytes > 0 && lastBytes < fileTotalBytes)
                        {
                            Interlocked.Add(ref totalBytesDownloaded, fileTotalBytes - lastBytes);
                        }

                        int finished = Interlocked.Increment(ref completedFiles);
                        progressReporter?.Report(
                            new DownloadByteProgressInfo(
                                totalFiles,
                                finished,
                                fileName,
                                fileTotalBytes,
                                fileTotalBytes,
                                Interlocked.Read(ref totalBytesDownloaded),
                                totalBytes,
                                0,
                                finished == totalFiles));
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 获取远程文件的 Content-Length（字节数），若无法获取则返回 0
        /// </summary>
        /// <param name="url">远程文件URL</param>
        /// <returns>文件字节数，获取失败返回0</returns>
        private static async Task<long> GetContentLengthAsync(string url)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, url))
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    return response.Content.Headers.ContentLength ?? 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        // 根据字节大小返回合适的单位值（KB, MB, GB等）
        public static (double value, string unit) BytesToUnit(long byteCount)
        {
            // 如果字节数小于 1024，直接返回字节
            if (byteCount < 1024)
                return (byteCount, "B");

            // 定义单位数组
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

            // 计算合适的单位索引
            int unitIndex = 0;
            double size = byteCount;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            // 返回保留两位小数的结果
            return (Math.Round(size, 2), units[unitIndex]);
        }
    }
}
