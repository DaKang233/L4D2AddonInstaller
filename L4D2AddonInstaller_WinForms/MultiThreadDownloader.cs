using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace L4D2AddonInstaller_WinForms
{
    /// <summary>
    /// HTTPS请求工具类（处理ZeroSSL证书，获取远程文件）
    /// </summary>
    public static class HttpHelper
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
        public static async Task DownloadFileAsync(string url, string savePath, IProgress<int> progress = null)
        {
            try
            {
                // 使用HttpClient发送异步GET请求，获取响应头信息后即开始接收内容
                using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    // 确保HTTP响应状态码为成功状态码（200-299）
                    response.EnsureSuccessStatusCode();

                    // 获取HTTP响应内容的总字节数，如果获取不到则设为0
                    var totalBytes = response.Content.Headers.ContentLength ?? 0;
                    var downloadedBytes = 0L; // 已下载的字节数初始化为0
                    var buffer = new byte[8192]; // 创建一个8KB的缓冲区用于读取和写入文件

                    // 异步读取HTTP响应内容的流
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    // 创建或打开文件以写入下载的数据
                    using (var fileStream = System.IO.File.Create(savePath))
                    {
                        int bytesRead; // 定义一个变量用于存储每次读取的字节数
                        // 循环读取HTTP响应流中的数据，直到没有更多数据可读（bytesRead <= 0）
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            // 将读取到的数据写入文件流
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            downloadedBytes += bytesRead; // 更新已下载的字节数

                            // 如果提供了进度回调接口且总字节数大于0，则计算下载进度百分比并报告
                            if (totalBytes > 0 && progress != null)
                            {
                                var percent = (int)((downloadedBytes * 100.0) / totalBytes); // 计算百分比
                                progress.Report(percent); // 报告进度
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"网络请求失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                // 抛出自定义异常，包含原始异常信息
                throw new Exception($"下载文件失败，请尝试联系开发者：{ex.Message}", ex);
            }
        }
    }
}
