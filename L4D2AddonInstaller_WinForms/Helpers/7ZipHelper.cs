using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L4D2AddonInstaller.Helper
{
    /// <summary>
    /// The exception that is thrown when an operation requires a password-protected archive to be accessed without
    /// providing a password.
    /// </summary>
    /// <remarks>This exception indicates that the archive cannot be opened or processed because it is
    /// protected by a password. Catch this exception to prompt the user for a password or to handle password-protected
    /// archives appropriately.</remarks>
    public class ArchiveRequiresPasswordException : Exception
    {
        // 无参构造函数
        public ArchiveRequiresPasswordException() : base("压缩包受密码保护，请输入密码！")
        {
        }

        // 带自定义消息的构造函数
        public ArchiveRequiresPasswordException(string message) : base(message)
        {
        }

        // 带消息和内部异常的构造函数（用于异常链）
        public ArchiveRequiresPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Provides helper methods for working with 7-Zip archives, including extraction, integrity validation, and
    /// encryption detection using the 7z.exe command-line tool.
    /// </summary>
    /// <remarks>This static class offers asynchronous methods to interact with 7-Zip archives by invoking the
    /// external 7z.exe utility. It supports specifying custom 7z.exe paths, handling password-protected archives,
    /// reporting extraction progress, and managing overwrite behaviors. All methods require 7z.exe to be available on
    /// the system. Callers should ensure that the appropriate version of 7z.exe is present and accessible. Methods may
    /// throw exceptions if the archive is invalid, the password is incorrect, or the operation is canceled.</remarks>
    public static class SevenZipHelper
    {
        /// <summary>
        /// 7-Zip 覆盖模式
        /// </summary>
        public enum OverwriteMode
        {
            OverwriteAll, // 覆盖所有（-y）
            SkipExisting, // 跳过已存在（-aos）
            RenameNewer,  // 重命名新文件（-aou）
            RenameExisting // 重命名已存在（-aot）
        }

        /// <summary>
        /// Gets the full path to the 7z.exe executable if it exists in a standard location.
        /// </summary>
        /// <remarks>This method searches for 7z.exe in several common locations, including the
        /// application's base directory, the current working directory, the Program Files directory, and a parent
        /// project tools directory. The search order determines which path is returned if multiple copies
        /// exist.</remarks>
        /// <returns>A string containing the full path to 7z.exe if found in a known directory; otherwise, null.</returns>
        public static string Default7ZipFullPath() {
            var candidatePaths = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, "tools", "7z.exe"),
                Path.Combine(AppContext.BaseDirectory, "7z.exe"),
                Path.Combine(Environment.CurrentDirectory, "7z.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe")
            };

            var currentDirectoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
            for (var i = 0; i < 2 && currentDirectoryInfo?.Parent != null; i++)
            {
                currentDirectoryInfo = currentDirectoryInfo.Parent;
            }

            if (currentDirectoryInfo != null)
            {
                candidatePaths.Add(Path.Combine(currentDirectoryInfo.FullName, "tools", "7z.exe"));
            }

            Debug.WriteLine($"检测 7z.exe 路径：\n{string.Join("\n", candidatePaths)}");

            return candidatePaths.FirstOrDefault(path => File.Exists(path));
        }

        /// <summary>
        /// 异步下载 7z.exe 和 7z.dll
        /// </summary>
        public static async Task Download7ZipExe(CancellationToken cancellationToken, IProgress<int> progress = null)
        {
            var sevenZipExeUrl = "https://furina.dakang233.com:8443/www/tools/7z.exe";
            var sevenZipDllUrl = "https://furina.dakang233.com:8443/www/tools/7z.dll";
            Uri sevenZipExeFinalUri = new Uri( sevenZipExeUrl );
            Uri sevenZipDllFinalUri = new Uri( sevenZipDllUrl );

            var sevenZipPath = Path.Combine(AppContext.BaseDirectory, "tools");
            Directory.CreateDirectory(sevenZipPath);

            if (File.Exists(Path.Combine(AppContext.BaseDirectory,"tools","7z.exe")) || File.Exists(Path.Combine(AppContext.BaseDirectory, "tools", "7z.dll")))
            {
                File.Delete(Path.Combine(AppContext.BaseDirectory, "tools", "7z.exe"));
                File.Delete(Path.Combine(AppContext.BaseDirectory, "tools", "7z.dll"));
            }
            try
            {
                var progress1 = new Progress<int>(p =>
                {
                    progress?.Report(p * 23 / 100);
                });
                var progress2 = new Progress<int>(p =>
                {
                    progress?.Report(p * 77 / 100 + 23);
                });
                await HttpHelper.DownloadFileAsync(sevenZipExeUrl, Path.Combine(sevenZipPath, "7z.exe"), cancellationToken, progress1);
                await HttpHelper.DownloadFileAsync(sevenZipDllUrl, Path.Combine(sevenZipPath, "7z.dll"), cancellationToken, progress2);
            }
            catch (Exception)
            {
                throw;
            }
        }
        

        /// <summary>
        /// 异步地将 7-Zip 归档文件的内容提取到指定的输出目录。
        /// </summary>
        /// <remarks>此方法使用 7-Zip 命令行工具执行提取操作。提取进度
        /// 基于 7-Zip 的输出来报告，可能并非对所有归档类型都可用。该方法
        /// 支持使用 includeFiles 参数从归档中提取特定的文件或匹配模式的文件。如果
        /// 输出目录不存在，将会自动创建。此方法是线程安全的，并且可以等待。</remarks>
        /// <param name="archivePath">要提取的 7-Zip 归档文件的完整路径。必须指向一个已存在的文件。</param>
        /// <param name="outputDirectory">提取的文件将被放置的目录。如果该目录不存在，则会创建。</param>
        /// <param name="sevenZipExe">用于提取操作的 7-Zip 可执行文件的完整路径。如果为 null 或为空，则使用
        /// 默认的 7-Zip 路径。必须指向一个有效的 7-Zip 可执行文件。</param>
        /// <param name="progress">一个可选的进度报告器，用于接收提取进度百分比（范围从 0 到 100）。如果不需要
        /// 报告进度，可以为 null。</param>
        /// <param name="password">用于提取加密归档文件的密码。如果为 null 或为空，则在没有密码的情况下
        /// 进行提取。</param>
        /// <param name="overwriteMode">指定如何处理输出目录中已存在的文件。决定在提取过程中是覆盖、跳过还是
        /// 重命名文件。</param>
        /// <param name="cancellationToken">可用于取消提取操作的取消令牌。</param>
        /// <param name="includeFiles">一个可选的数组，包含要从归档中提取的文件名或匹配模式。如果未指定或为空，则提取
        /// 所有文件。</param>
        /// <returns>表示异步提取操作的任务。</returns>
        /// <exception cref="FileNotFoundException">当 archivePath 指定的归档文件不存在，或者找不到有效的 7-Zip 可执行文件时抛出。</exception>
        /// <exception cref="Exception">当提取过程失败或 7-Zip 进程返回非零退出代码时抛出。</exception>
        /// <exception cref="OperationCanceledException">当提取操作通过 cancellationToken 被取消时抛出。</exception>
        
        /// <summary>
        /// Extracts the contents of a 7-Zip archive to the specified output directory asynchronously.
        /// </summary>
        /// <remarks>This method uses the 7-Zip command-line tool to perform extraction. Extraction
        /// progress is reported based on 7-Zip's output, and may not be available for all archive types. The method
        /// supports extracting specific files or patterns from the archive using the includeFiles parameter. If the
        /// output directory does not exist, it is created automatically. This method is thread-safe and can be
        /// awaited.</remarks>
        /// <param name="archivePath">The full path to the 7-Zip archive file to extract. Must refer to an existing file.</param>
        /// <param name="outputDirectory">The directory where the extracted files will be placed. The directory is created if it does not exist.</param>
        /// <param name="sevenZipExe">The full path to the 7-Zip executable to use for extraction. If null or empty, the default 7-Zip path is
        /// used. Must refer to a valid 7-Zip executable.</param>
        /// <param name="progress">An optional progress reporter that receives extraction progress as a percentage from 0 to 100. Can be null
        /// if progress reporting is not required.</param>
        /// <param name="password">The password to use for extracting encrypted archives. If null or empty, extraction proceeds without a
        /// password.</param>
        /// <param name="overwriteMode">Specifies how to handle existing files in the output directory. Determines whether to overwrite, skip, or
        /// rename files during extraction.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the extraction operation.</param>
        /// <param name="includeFiles">An optional array of file names or patterns to extract from the archive. If not specified or empty, all
        /// files are extracted.</param>
        /// <returns>A task that represents the asynchronous extraction operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the archive file specified by archivePath does not exist, or if a valid 7-Zip executable cannot be
        /// found.</exception>
        /// <exception cref="Exception">Thrown if the extraction process fails or the 7-Zip process returns a non-zero exit code.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the extraction operation is canceled via the cancellationToken.</exception>
        public static async Task ExtractAsync(
            string archivePath,
            string outputDirectory,
            string sevenZipExe = null,
            IProgress<int> progress = null,
            string password = null,
            OverwriteMode overwriteMode = OverwriteMode.OverwriteAll,
            CancellationToken cancellationToken = default,
            params string[] includeFiles)
        {
            if (!File.Exists(archivePath))
                throw new FileNotFoundException("压缩文件不存在", archivePath);

            Directory.CreateDirectory(outputDirectory);

            // 7-Zip 程序路径
            if (string.IsNullOrEmpty(sevenZipExe))
                sevenZipExe = Default7ZipFullPath();
            if (string.IsNullOrEmpty(sevenZipExe))
                throw new FileNotFoundException("未找到有效的 7-Zip 程序", sevenZipExe);

            // 构造参数
            var argsBuilder = new StringBuilder($"x \"{archivePath}\" -o\"{outputDirectory}\" -bsp1");

            if (!string.IsNullOrEmpty(password))
            {
                argsBuilder.Append($" -p\"{password}\"");
            }
            switch (overwriteMode)
            {
                case OverwriteMode.OverwriteAll: argsBuilder.Append(" -y"); break;
                case OverwriteMode.SkipExisting: argsBuilder.Append(" -aos"); break;
                case OverwriteMode.RenameNewer: argsBuilder.Append(" -aou"); break;
                case OverwriteMode.RenameExisting: argsBuilder.Append(" -aot"); break;
            }
            if (includeFiles != null && includeFiles.Length > 0)
            {
                argsBuilder.Append(" ");
                argsBuilder.Append(string.Join(" ", includeFiles.Select(f => $"\"{f}\"")));
            }

            var arguments = argsBuilder.ToString();

            // 配置进程启动信息
            var psi = new ProcessStartInfo
            {
                FileName = sevenZipExe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Universal.TryGetGB18030Encoding(),
                StandardErrorEncoding = Universal.TryGetGB18030Encoding()
            };

            // 启动进程并处理输出
            using ( var process = new Process { StartInfo = psi })
            using (cancellationToken.Register(() => // 注册取消回调：触发时杀死进程
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill(); // 强制终止7z进程
                            Debug.WriteLine("解压进程已被手动终止");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"终止进程时发生错误：{ex.Message}");
                    }
                }))
            {
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (_, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data)) return;
                    outputBuilder.AppendLine(e.Data);
                    Debug.WriteLine($"7z解压日志: {e.Data}");
                    // 解析7-Zip的进度输出（格式如"10%"）
                    var data = e.Data.Trim();
                    if (data.Contains("%"))
                    {
                        // 从包含"%"的字符串中提取数字（忽略其他字符）
                        var percentStr = new string(data.Where(c => char.IsDigit(c)).ToArray());
                        if (int.TryParse(percentStr, out int percent))
                        {
                            // 确保进度在0-100范围内
                            percent = Math.Min(percent, 100);
                            progress?.Report(percent);
                        }
                    }
                };
                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        errorBuilder.AppendLine(e.Data);
                    var data = e.Data?.Trim();
                    Debug.WriteLine($"7z解压错误日志: {e.Data}");
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await ProcessHelper.WaitForExitAsync(process);

                if (process.ExitCode != 0)
                {
                    throw new Exception(
                        $"7-Zip 解压失败 (ExitCode={process.ExitCode})\n" +
                        errorBuilder.ToString()
                    );
                }
                else if (cancellationToken.IsCancellationRequested) {
                    progress?.Report(0);
                    throw new OperationCanceledException("解压操作已被取消");
                }

                progress?.Report(100); // 确保进度达到100%
            }
        }

        /// <summary>
        /// Validates the integrity of a 7z archive file using the 7-Zip command-line tool. Throws an exception if the
        /// archive is corrupted, the password is incorrect, or the archive cannot be opened.
        /// </summary>
        /// <remarks>This method uses the external 7-Zip tool to perform archive validation. The operation
        /// runs asynchronously and may take time depending on the size of the archive. Ensure that the specified 7z.exe
        /// is compatible with the archive format. The method does not extract the archive contents; it only checks for
        /// integrity and password correctness.</remarks>
        /// <param name="archivePath">The full path to the 7z archive file to validate. Cannot be null or empty.</param>
        /// <param name="sevenZipExe">The path to the 7z.exe executable to use for validation. If null, defaults to a bundled 7z.exe in the
        /// application's tools directory.</param>
        /// <param name="password">The password to use for opening the archive. Specify an empty string if the archive is not
        /// password-protected.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the validation operation.</param>
        /// <returns>A task that represents the asynchronous validation operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the specified 7z.exe executable cannot be found.</exception>
        /// <exception cref="ArchiveRequiresPasswordException">Thrown if the archive is encrypted and the provided password is incorrect or missing.</exception>
        /// <exception cref="Exception">Thrown if the archive is corrupted or if an error occurs during validation that is not related to password
        /// protection.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the provided cancellation token.</exception>
        public static async Task ValidateArchiveAsync(
            string archivePath, 
            string sevenZipExe, 
            string password, 
            CancellationToken cancellationToken = default)
        {
            sevenZipExe = sevenZipExe ?? Path.Combine(AppContext.BaseDirectory, "tools", "7z.exe");
            if (string.IsNullOrEmpty(sevenZipExe) ) {
                throw new FileNotFoundException("无效的 7z.exe 路径", sevenZipExe);
            }

            var argsBuilder = new StringBuilder($"t \"{archivePath}\"");
            if (!string.IsNullOrEmpty(password))
                argsBuilder.Append($" -p{password}");
            else
                argsBuilder.Append($" -p\"\"");
            string arguments = argsBuilder.ToString();

            var psi = new ProcessStartInfo
            {
                FileName = sevenZipExe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                StandardOutputEncoding = Universal.TryGetGB18030Encoding(),
                StandardErrorEncoding = Universal.TryGetGB18030Encoding()
            };

            using (var process = new Process { StartInfo = psi })
            using (cancellationToken.Register(() => // 注册取消回调：触发时杀死进程
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(); // 强制终止7z进程
                        Debug.WriteLine("解压进程已被手动终止");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"终止进程时发生错误：{ex.Message}");
                }
            }))
            {
                var errorBuilder = new StringBuilder();
                var outputBuilder = new StringBuilder();
                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorBuilder.AppendLine(e.Data);
                        Debug.WriteLine($"7z验证错误日志: {e.Data}");
                    }
                };

                process.OutputDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine(e.Data);
                        Debug.WriteLine($"7z验证日志: {e.Data}");
                        var data = e.Data?.Trim();
                        if (!string.IsNullOrWhiteSpace(data) && data.Contains("Everything is Ok"))
                        {
                            Debug.WriteLine("压缩包完整性验证通过");
                        }
                    }
                };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                await ProcessHelper.WaitForExitAsync(process);
                if (process.ExitCode != 0)
                {
                    if (errorBuilder.ToString().Contains("Cannot open encrypted archive. Wrong password?"))
                        throw new ArchiveRequiresPasswordException();
                    throw new Exception($"压缩包已损坏或密码错误 (ExitCode={process.ExitCode})\n{errorBuilder}");
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException("压缩包验证操作已被取消");
                }
            }
        }

        /// <summary>
        /// Asynchronously determines whether the specified archive file is encrypted by invoking the 7-Zip command-line
        /// tool.
        /// </summary>
        /// <remarks>This method relies on the output of the specified 7-Zip executable to detect
        /// encryption. Ensure that the provided 7-Zip version supports the required command-line options. The method
        /// does not attempt to decrypt the archive; it only checks for the presence of encryption.</remarks>
        /// <param name="archivePath">The full path to the archive file to check for encryption. This value cannot be null or empty.</param>
        /// <param name="sevenZipExe">The full path to the 7-Zip executable to use for inspecting the archive. This value cannot be null or empty.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the archive
        /// is encrypted; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the <paramref name="cancellationToken"/> parameter.</exception>
        public static async Task<bool> IsArchiveEncryptedAsync(
            string archivePath,
            string sevenZipExe,
            CancellationToken cancellationToken)
        {
            // -l：列出内容，-slt：显示详细的技术信息，-ba：仅输出纯数据（屏蔽进度）
            string arguments = $"l -slt -ba \"{archivePath}\" -p \"\"";

            var psi = new ProcessStartInfo
            {
                FileName = sevenZipExe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Universal.TryGetGB18030Encoding(),
                StandardErrorEncoding = Universal.TryGetGB18030Encoding()
            };

            bool isEncrypted = false;

            using (var process = new Process { StartInfo = psi })
            using (cancellationToken.Register(() =>
            {
                try { if (!process.HasExited) process.Kill(); }
                catch { }
            }))
            {
                process.OutputDataReceived += (_, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data)) return;
                    var data = e.Data.Trim();
                    Debug.WriteLine($"7z加密检测日志: {e.Data}");
                    // 检测7z输出的加密标记（关键：Encrypted = +）
                    if (data.StartsWith("Encrypted = +"))
                    {
                        isEncrypted = true;
                    }
                };

                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.WriteLine($"7z加密检测错误日志: {e.Data}");
                        var data = e.Data.Trim();
                        if (data.StartsWith("Cannot open encrypted archive. Wrong password?"))
                        {
                            isEncrypted = true;
                        }
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await ProcessHelper.WaitForExitAsync(process);

                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException("加密检测操作已被取消");
                }
            }

            return isEncrypted;
        }
    }
}
