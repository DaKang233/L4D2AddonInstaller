using InstallerForL4D2AddonInstaller.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static InstallerForL4D2AddonInstaller.Parser.SteamLibraryVdfParser;

namespace InstallerForL4D2AddonInstaller
{
    public partial class InstallProgressStep : UserControl
    {
        private InstallerForm installerForm;

        public InstallProgressStep(InstallerForm form)
        {
            InitializeComponent();
            installerForm = form ?? throw new ArgumentNullException(nameof(form));
        }

        public event EventHandler<bool> InstallationCompleted;

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var versionDetails = installerForm?.SelectedVersionDetail;
            if (versionDetails == null)
            {
                MessageBox.Show("未选择版本或版本信息未加载，请重启程序。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                installerForm.IsClosedWithoutAsking = true;
                installerForm.Close();
            }

            await InstallAsync(versionDetails);
        }

        private void AppendLog(string message)
        {
            richTextBoxLog.AppendText($"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}\n");
        }

        private async Task InstallAsync(VersionDetails versionDetails)
        {
            var downloadArchivePath = versionDetails.RelativePath;
            var archiveFileName = Path.GetFileName(downloadArchivePath);
            var prefix = versionDetails.Prefix;
            var webServer = versionDetails.WebServer;
            var webPort = versionDetails.WebPort;
            var protocol = versionDetails.Protocol;
            var installPath = Path.Combine(installerForm.InstallPath,InstallerForm.InstallerAppName);
            installerForm.cts = new System.Threading.CancellationTokenSource();
            var cancellationToken = installerForm.cts.Token;

            progressBar.Value = 0;
            AppendLog("开始下载 7-Zip 组件...");
            try
            {
                await Helper.SevenZipHelper.Download7ZipExeToDirectory(cancellationToken, installPath, progress: new Progress<int>(value =>
                {
                    if (value == 100)
                    {
                        labelStatus.Text = "7-Zip 下载完成。";
                        AppendLog($"{labelStatus.Text}");
                    }
                    if (value <= 23)
                    {
                        labelStatus.Text = "正在下载 7z.exe...";
                    }
                    if (value > 23 && value < 100)
                    {
                        labelStatus.Text = "正在下载 7z.dll...";
                    }
                    progressBar.Value = Math.Min(value,100);
                }));
            }
            catch (Exception ex)
            {
                AppendLog(ex.ToString());
                MessageBox.Show($"下载 7-Zip 组件时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                installerForm.IsClosedWithoutAsking = true;
                installerForm.Close();
                return;
            }

            var downloadList = HttpHelper.GetDownloadList(protocol, webServer, webPort, prefix, new List<string> { downloadArchivePath }, installPath);
            progressBar.Value = 0;
            AppendLog("开始下载程序...");

            int lastLoggedPercent = -1;
            DateTime lastLogTime = DateTime.MinValue;
            TimeSpan logInterval = TimeSpan.FromMilliseconds(800);

            try
            {
                await HttpHelper.DownloadListItemsWithByteProgressAsync(downloadList.Items, cancellationToken, progressReporter: new Progress<HttpHelper.DownloadByteProgressInfo>(progressInfo =>
                {
                    if (progressInfo.IsCompleted)
                    {
                        labelStatus.Text = "文件下载完成。";
                        AppendLog($"{labelStatus.Text}");
                        progressBar.Value = 100;
                    }
                    else
                    {
                        long totalBytesSafe = progressInfo.TotalBytes > 0 ? progressInfo.TotalBytes : progressInfo.CurrentFileTotalBytes;
                        long downloadedBytesSafe = progressInfo.TotalBytes > 0 ? progressInfo.TotalBytesDownloaded : progressInfo.CurrentFileBytesDownloaded;

                        int totalPercent = 0;
                        if (totalBytesSafe > 0)
                        {
                            totalPercent = (int)((double)downloadedBytesSafe / totalBytesSafe * 100);
                        }

                        var (speedValue, speedUnit) = HttpHelper.BytesToUnit((long)Math.Max(progressInfo.CurrentFileSpeedBytesPerSec, 0));
                        labelStatus.Text = $"正在下载 {progressInfo.CurrentFileName} ({HttpHelper.GetBytesUnitString(progressInfo.CurrentFileBytesDownloaded)}/{HttpHelper.GetBytesUnitString(progressInfo.CurrentFileTotalBytes)})";
                        progressBar.Value = Math.Max(Math.Min(totalPercent,100),0);
                        labelSpeed.Text = $"{speedValue:F2} {speedUnit}/s";

                        var now = DateTime.Now;
                        if (totalPercent != lastLoggedPercent && (now - lastLogTime >= logInterval || totalPercent == 100))
                        {
                            AppendLog($"正在下载文件：{progressInfo.CurrentFileName} ({totalPercent}%)");
                            lastLoggedPercent = totalPercent;
                            lastLogTime = now;
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                AppendLog(ex.ToString());
                MessageBox.Show($"下载文件时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (System.IO.File.Exists(System.IO.Path.Combine(installPath, downloadArchivePath)))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(installPath, downloadArchivePath));
                }
                installerForm.IsClosedWithoutAsking = true;
                installerForm.Close();
                return;
            }

            labelSpeed.Text = string.Empty;
            progressBar.Value = 0;
            try
            {
                var archiveFullPath = System.IO.Path.Combine(installPath, archiveFileName);
                var progress = new Progress<int>(value =>
                {
                    labelStatus.Text = $"正在解压文件... ({value}%)";
                    AppendLog($"正在解压文件：{archiveFileName} ({value}%)");
                    progressBar.Value = Math.Max(Math.Min(value,100),0);
                });
                await Helper.SevenZipHelper.ExtractAsync(
                    archiveFullPath,
                    installPath,
                    Path.Combine(installPath,"tools","7z.exe"),
                    progress,
                    null,   // No password
                    SevenZipHelper.OverwriteMode.OverwriteAll,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                AppendLog(ex.ToString());
                MessageBox.Show($"解压文件时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string fullPath = Path.GetFullPath(installPath);

                if (Directory.Exists(fullPath))
                {
                    Universal.DeleteDirectoryFiles(fullPath);
                }
                else if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                installerForm.IsClosedWithoutAsking = true;
                installerForm.Close();
                return;
            }
            progressBar.Value = 100;
            AppendLog("解压完成。");
            labelStatus.Text = "安装完成。";
            InstallationCompleted?.Invoke(this, true);
        }
    }
}
