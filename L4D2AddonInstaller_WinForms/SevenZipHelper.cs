using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace L4D2AddonInstaller_WinForms
{
    /// <summary>
    /// 解压缩文件（7z / zip / rar 等）
    /// </summary>
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
        /// 此方法返回默认的 7z.exe 路径（如果存在）
        /// </summary>
        /// <returns>7z.exe 的完整路径</returns>
        public static string Default7ZipFullPath() {
            var sevenZipPath = Path.Combine(
                AppContext.BaseDirectory,
                "tools",
                "7z.exe"
            );
            var sevenZipCurrentDirPath = Path.Combine(
                Environment.CurrentDirectory,
                "7z.exe"
            );
            var sevenZipProgramFilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "7-Zip",
                "7z.exe"
            );
            var sevenZipVSProjectPath = Path.Combine(
                Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName,
                "tools",
                "7z.exe");
            Debug.WriteLine($"检测 7z.exe 路径：\n{sevenZipPath}\n{sevenZipCurrentDirPath}\n{sevenZipProgramFilesPath}\n{sevenZipVSProjectPath}");
            if (File.Exists(sevenZipPath)) {
                return sevenZipPath;
            }
            else if (File.Exists(sevenZipCurrentDirPath)) {
                return sevenZipCurrentDirPath;
            }
            else if (File.Exists(sevenZipProgramFilesPath)) {
                return sevenZipProgramFilesPath;
            }
            else if (File.Exists(sevenZipVSProjectPath)) {
                return sevenZipVSProjectPath;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// 异步解压缩文件到指定目录
        /// </summary>
        /// <param name="archivePath">压缩包路径</param>
        /// <param name="outputDirectory">输出目录</param>
        /// <param name="sevenZipExe">可选的 7z.exe 路径</param>
        /// <param name="progress">进度条</param>
        /// <param name="password">密码</param>
        /// <param name="overwriteMode">覆盖模式</param>
        /// <param name="includeFiles">筛选解压的文件</param>
        /// <returns>表示解压缩已完成或出错的任务</returns>
        /// <exception cref="FileNotFoundException">压缩文件不存在/7z.exe 路径无效</exception>
        /// <exception cref="Exception">解压失败</exception>
        public static async Task ExtractAsync(
            string archivePath,
            string outputDirectory,
            string sevenZipExe = null,
            IProgress<int> progress = null,
            string password = null,
            OverwriteMode overwriteMode = OverwriteMode.OverwriteAll,
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

            // 验证压缩包完整性
            try
            {
                await ValidateArchiveAsync(archivePath, sevenZipExe, password);
            }
            catch (Exception ex)
            {
                throw new Exception("压缩包验证失败", ex);
            }

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
            {
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (_, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data)) return;
                    outputBuilder.AppendLine(e.Data);
                    // 解析7-Zip的进度输出（格式如"10%"）
                    var data = e.Data.TrimEnd();
                    if (data.EndsWith("%") && int.TryParse(data.TrimEnd('%'), out int percent))
                    {
                        progress?.Report(percent); // 触发进度回调
                    }
                };
                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        errorBuilder.AppendLine(e.Data);
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

                progress?.Report(100); // 确保进度达到100%
            }
        }

        /// <summary>
        /// 一个异步方法，用于验证压缩包的完整性
        /// </summary>
        /// <param name="archivePath">压缩包的文件路径</param>
        /// <param name="sevenZipExe">7z.exe 程序路径</param>
        /// <param name="password">可选的压缩包密码</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task ValidateArchiveAsync(string archivePath, string sevenZipExe, string password)
        {
            sevenZipExe = sevenZipExe ?? Path.Combine(AppContext.BaseDirectory, "tools", "7z.exe");
            if (string.IsNullOrEmpty(sevenZipExe) ) {
                throw new FileNotFoundException("无效的 7z.exe 路径", sevenZipExe);
            }

            var argsBuilder = new StringBuilder($"t \"{archivePath}\"");
            if (!string.IsNullOrEmpty(password))
                argsBuilder.Append($" -p{password}");
            string arguments = argsBuilder.ToString();

            var psi = new ProcessStartInfo
            {
                FileName = sevenZipExe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.GetEncoding("GB18030")
            };

            using (var process = new Process { StartInfo = psi })
            {
                var errorBuilder = new StringBuilder();
                process.ErrorDataReceived += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) errorBuilder.AppendLine(e.Data); };
                process.Start();
                process.BeginErrorReadLine();
                await ProcessHelper.WaitForExitAsync(process);
                if (process.ExitCode != 0)
                {
                    throw new Exception($"压缩包已损坏或密码错误 (ExitCode={process.ExitCode})\n{errorBuilder}");
                }
            }
        }
    }
}
