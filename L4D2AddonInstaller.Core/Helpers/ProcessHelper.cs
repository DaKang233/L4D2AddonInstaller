using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace L4D2AddonInstaller.Helpers
{
    /// <summary>
    /// 进程相关操作
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// 判断指定进程是否正在运行
        /// </summary>
        public static bool IsProcessRunning(string processName)
        {
            try
            {
                return Process.GetProcessesByName(processName).Any();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 启动一个可执行文件（如 steam.exe、7z.exe）
        /// </summary>
        public static void StartExecutable(string exePath)
        {
            if (string.IsNullOrWhiteSpace(exePath))
                throw new ArgumentException("可执行文件路径为空");

            if (!File.Exists(exePath))
                throw new FileNotFoundException("未找到可执行文件", exePath);

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 启动一个协议 URL（如 steam://）
        /// </summary>
        public static void StartUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("URI 不能为空");

            Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 异步启动协议 URL（避免阻塞 UI）
        /// </summary>
        public static Task StartUriAsync(string uri)
        {
            return Task.Run(() => StartUri(uri));
        }

        /// <summary>
        /// 异步等待一个进程退出。
        /// </summary>
        /// <param name="process">要等待退出的进程。</param>
        /// <returns>一个任务，当进程退出时完成。</returns>
        public static async Task WaitForExitAsync(Process process)
        {
            // 创建一个任务完成源来表示进程退出的完成情况。
            var tcs = new TaskCompletionSource<object>();

            // 启用进程的事件引发，以便可以监听进程退出事件。
            process.EnableRaisingEvents = true;

            // 添加进程退出事件的处理程序，当进程退出时将任务完成源设置为结果。
            process.Exited += (sender, args) => tcs.SetResult(null);

            // 异步等待任务完成，即等待进程退出。
            await tcs.Task;
        }

    }
}
