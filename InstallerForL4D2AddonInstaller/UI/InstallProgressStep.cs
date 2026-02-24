using InstallerForL4D2AddonInstaller.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static InstallerForL4D2AddonInstaller.Parser.SteamLibraryVdfParser;

namespace InstallerForL4D2AddonInstaller
{
    public partial class InstallProgressStep : UserControl
    {
        private readonly InstallerForm installerForm;
        private readonly IInstallService installService;

        public InstallProgressStep(InstallerForm form)
            : this(form, new InstallService())
        {
        }

        internal InstallProgressStep(InstallerForm form, IInstallService installService)
        {
            InitializeComponent();
            installerForm = form ?? throw new ArgumentNullException(nameof(form));
            this.installService = installService ?? throw new ArgumentNullException(nameof(installService));
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
                return;
            }

            await InstallAsync(versionDetails);
        }

        private void AppendLog(string message)
        {
            richTextBoxLog.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}\n");
        }

        private async Task InstallAsync(VersionDetails versionDetails)
        {
            installerForm.cts = new CancellationTokenSource();
            CancellationToken cancellationToken = installerForm.cts.Token;

            progressBar.Value = 0;
            var progress = new Progress<InstallProgressInfo>(info =>
            {
                labelStatus.Text = info.StatusMessage ?? string.Empty;
                progressBar.Value = Math.Max(0, Math.Min(100, info.Percent));

                if (info.Stage == InstallStage.DownloadingPackage)
                {
                    var (speedValue, speedUnit) = Helper.HttpHelper.BytesToUnit((long)Math.Max(info.SpeedBytesPerSecond, 0));
                    labelSpeed.Text = $"{speedValue:F2} {speedUnit}/s";
                }
                else if (info.Stage == InstallStage.Completed || info.Stage == InstallStage.Extracting)
                {
                    labelSpeed.Text = string.Empty;
                }

                AppendLog(info.StatusMessage ?? string.Empty);
            });

            try
            {
                await installService.InstallAsync(installerForm.InstallPath, versionDetails, progress, cancellationToken);
                InstallationCompleted?.Invoke(this, true);
            }
            catch (OperationCanceledException)
            {
                AppendLog("安装已取消。");
                labelStatus.Text = "安装已取消。";
            }
            catch (Exception ex)
            {
                AppendLog(ex.ToString());
                MessageBox.Show($"安装时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                var installPath = Path.Combine(installerForm.InstallPath, InstallerForm.InstallerAppName);
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
            }
        }
    }
}
