using L4D2AddonInstaller.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static L4D2AddonInstaller.Helpers.SevenZipHelper;
using static L4D2AddonInstaller.MainForm;

namespace L4D2AddonInstaller
{
    /// <summary>
    /// Represents a Windows Form that provides a graphical interface for extracting archive files using 7-Zip. Allows
    /// users to select archives, specify extraction options, and monitor extraction progress with support for multiple
    /// files and cancellation.
    /// </summary>
    /// <remarks>SevenZipForm enables users to extract one or more supported archive files (such as .7z, .zip,
    /// .rar, etc.) to a specified output directory using a chosen 7-Zip executable. The form validates user input,
    /// displays progress and status updates, and allows cancellation of ongoing extraction operations. Extraction can
    /// be triggered manually or automatically via an ExtractRequest. The ExtractionCompleted event notifies subscribers
    /// when the extraction process finishes, indicating success or failure. This form is not thread-safe and should be
    /// used only on the UI thread.</remarks>
    public partial class SevenZipForm : Form
    {

        private CancellationTokenSource _cts;
        private readonly ExtractRequest _request;

        /// <summary>
        /// Initializes a new instance of the SevenZipForm class, optionally pre-populating fields based on the
        /// specified extract request.
        /// </summary>
        /// <remarks>If the provided request is not null and its IsAutoExtract property is true, the form
        /// fields are initialized with the values from the request. Otherwise, default values from the main form are
        /// used.</remarks>
        /// <param name="request">An optional extract request containing initial values for extraction settings. If null, default values are
        /// used.</param>
        public SevenZipForm(ExtractRequest request = null)
        {
            InitializeComponent();
            _request = request;
            InitOverrideModeRadio();
            if (request != null && request.IsAutoExtract)
            {
                textBox7ZipPath.Text = request.SevenZipPath;
                textBoxOutputDir.Text = request.OutputDirPath;
                textBoxArchivePath.Text = request.ArchivePath;
                textBoxIncludeFiles.Text = request.IncludeFiles;
            }
            else
            {
                textBox7ZipPath.Text = MainForm.SevenZipPath;
                textBoxOutputDir.Text = MainForm.OutputDirPath;
                textBoxArchivePath.Text = MainForm.ArchivePath;
            }
        }
        public event EventHandler<bool> ExtractionCompleted;
        public event EventHandler<bool> Download7ZipCompleted;



        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_request == null)
            {
                MainForm.overwriteMode = GetoverwriteModeRadio();
                MainForm.ArchivePath = textBoxArchivePath.Text;
                MainForm.OutputDirPath = textBoxOutputDir.Text;
                MainForm.SevenZipPath = textBox7ZipPath.Text;
            }
            base.OnFormClosing(e);
        }

        protected override void OnShown(EventArgs e)
        {
            if (_request != null && _request.IsAutoExtract)
            {
                if (!string.IsNullOrEmpty(_request.SevenZipPath) && System.IO.File.Exists(_request.SevenZipPath))
                    buttonStart.PerformClick();
                else
                {
                    buttonDownload7Zip.PerformClick();
                    Download7ZipCompleted += (s, success) =>
                    {
                        if (success)
                        {
                            buttonStart.PerformClick();
                        }
                        else
                        {
                            MessageBox.Show("一键操作失败：无法找到可用的 7-Zip 程序且下载 7-Zip 失败。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            RecoverUIAfter7ZipOperation(null);
                        }
                    };
                }
            }
            base.OnShown(e);
        }

        private void buttonArchiveBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择压缩包文件";
                openFileDialog.Filter = "压缩包文件|*.7z;*.zip;*.rar;*.tar;*.gz;*.bz2;*.xz;*.iso;*.cab;*.arj;*.lzh;*.z|所有文件|*.*";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxArchivePath.Text = string.Join(";", openFileDialog.FileNames);
                }
            }
        }

        private void button7ZipPathBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择 7z.exe 文件";
                openFileDialog.Filter = "7z 可执行程序|7z.exe|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox7ZipPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void button7ZipPathDetect_Click(object sender, EventArgs e)
        {
            textBox7ZipPath.Text = Default7ZipFullPath();
            if (string.IsNullOrEmpty(textBox7ZipPath.Text))
            {
                DialogResult result = MessageBox.Show("未在本地计算机上找到有效的 7-Zip 程序。是否尝试让程序下载？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    buttonDownload7Zip.PerformClick();
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Output Directory Detect button, validating the selected game directory and
        /// updating the output directory path accordingly.
        /// </summary>
        /// <remarks>If the game directory is not specified or does not contain the required executable,
        /// an error message is displayed and the output directory is not updated.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void buttonOutputDirDetect_Click(object sender, EventArgs e)
        {
            var gamePath = MainForm.GamePath;
            if (string.IsNullOrEmpty(gamePath) || !File.Exists(Path.Combine(gamePath, "left4dead2.exe")))
            {
                MessageBox.Show("请先指定有效的游戏目录路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string addonsPath = System.IO.Path.Combine(gamePath, "left4dead2", "addons");
            textBoxOutputDir.Text = addonsPath;
        }

        private void buttonOpenOutputDir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxOutputDir.Text))
            {
                MessageBox.Show("输出目录路径为空，无法打开。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "explorer.exe";
                process.StartInfo.Arguments = "\"" + textBoxOutputDir.Text + "\"";
                process.Start();
            }
        }

        private void buttonOutputDirBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "选择输出目录";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxOutputDir.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        // 本地函数：恢复 UI 状态（只能在最外层调用）
        void RecoverUIAfter7ZipOperation(string StatusText)
        {
            if (!string.IsNullOrEmpty(StatusText))
                labelStatus.Text = StatusText;
            buttonCancel.Enabled = false;
            buttonStart.Enabled = true;
            buttonDownload7Zip.Enabled = true;
            _cts?.Dispose();
        }

        /// <summary>
        /// Handles the Click event of the Start button to begin extracting one or more archive files using the
        /// specified 7-Zip executable and output directory.
        /// </summary>
        /// <remarks>Performs validation of user input before starting the extraction process. Updates the
        /// user interface to reflect progress and status, and supports cancellation. Displays error messages for
        /// invalid input or extraction failures. Extraction progress is reported for each archive, and the operation
        /// can be cancelled by the user.</remarks>
        /// <param name="sender">The source of the event, typically the Start button.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private async void buttonStart_Click(object sender, EventArgs e)
        {
            // 前置检查
            if (string.IsNullOrEmpty(textBox7ZipPath.Text) || !System.IO.File.Exists(textBox7ZipPath.Text))
            {
                MessageBox.Show("请指定有效的 7z.exe 的路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var archives = textBoxArchivePath.Text.Trim().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var archive in archives)
            {
                if (!System.IO.File.Exists(archive))
                {
                    MessageBox.Show($"压缩包文件不存在：{archive}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (string.IsNullOrEmpty(textBoxOutputDir.Text) || !System.IO.Directory.Exists(textBoxOutputDir.Text))
            {
                MessageBox.Show("请指定有效的输出目录路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 开始解压：UI 状态更新，进度条绑定，初始化变量
            labelPercent.Text = "0%";
            var overwriteMode = GetoverwriteModeRadio();
            labelStatus.Text = $"正在解压文件：{Path.GetFileName(textBoxArchivePath.Text)}...";
            buttonCancel.Enabled = true;
            buttonStart.Enabled = false;
            _cts = new CancellationTokenSource();
            var cancellationToken = _cts.Token;
            bool success = false;
            buttonDownload7Zip.Enabled = false;

            int archiveCount = archives.Length;
            try
            {
                for (int i = 0; i < archiveCount; i++)
                {
                    int index = i;
                    var perArchiveProgress = new Progress<int>(value =>
                    {
                        int totalPercent = (int)((index + value / 100.0) / archiveCount * 100);
                        progressBarCompression.Value = Math.Min(totalPercent, 100);
                        labelPercent.Text = $"{totalPercent}%";
                    });
                    labelStatus.Text = $"正在解压文件 {index + 1} / {archiveCount} ：{Path.GetFileName(archives[index])}...";
                    await ExtractOneArchiveAsync(
                        archives[index],
                        perArchiveProgress,
                        overwriteMode,
                        cancellationToken
                        );
                }
                success = true;
                ExtractionCompleted?.Invoke(this, true);
                if (_request != null && _request.IsAutoExtract)
                {
                    this.Close();
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("加密检验操作已被取消。", "已取消", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RecoverUIAfter7ZipOperation("解压已取消。");
                ExtractionCompleted?.Invoke(this, false);
                return;
            }
            catch (ArchiveRequiresPasswordException)
            {
                MessageBox.Show("该压缩包需要密码才能解压。请在密码框中输入正确的密码后重试。", "需要密码", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RecoverUIAfter7ZipOperation("解压失败，压缩包需要密码。");
                ExtractionCompleted?.Invoke(this, false);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("解压过程中发生错误：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RecoverUIAfter7ZipOperation("解压失败，发生错误。");
                ExtractionCompleted?.Invoke(this, false);
                return;
            }
            finally
            {
                if (success) RecoverUIAfter7ZipOperation($"全部压缩包（共 {archiveCount} 个）已解压到指定目录。");
            }
        }

        /// <summary>
        /// Extracts the contents of a single archive file asynchronously, reporting progress and handling password
        /// protection and overwrite options.
        /// </summary>
        /// <param name="archivePath">The full path to the archive file to extract. Cannot be null or empty.</param>
        /// <param name="progress">An optional progress reporter that receives extraction progress updates as percentage values. May be null if
        /// progress reporting is not required.</param>
        /// <param name="overwriteMode">Specifies how existing files should be handled during extraction. Determines whether to overwrite, skip, or
        /// prompt for existing files.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the extraction operation.</param>
        /// <returns>A task that represents the asynchronous extraction operation.</returns>
        /// <exception cref="ArchiveRequiresPasswordException">Thrown if the archive is encrypted and no password is provided.</exception>
        private async Task ExtractOneArchiveAsync(
                string archivePath,
                IProgress<int> progress,
                OverwriteMode overwriteMode,
                CancellationToken cancellationToken)
        {
            // 执行解压操作
            if (string.IsNullOrEmpty(textBoxPassword.Text))
            {
                if (await IsArchiveEncryptedAsync(archivePath, textBox7ZipPath.Text, cancellationToken))
                    throw new ArchiveRequiresPasswordException();
            }
            await SevenZipHelper.ValidateArchiveAsync(
                archivePath,
                textBox7ZipPath.Text,
                textBoxPassword.Text ?? null,
                cancellationToken
                );
            await SevenZipHelper.ExtractAsync(
                archivePath,
                textBoxOutputDir.Text,
                textBox7ZipPath.Text,
                progress,
                textBoxPassword.Text ?? null,
                overwriteMode,
                cancellationToken,
                !string.IsNullOrEmpty(textBoxIncludeFiles.Text)
                    ? textBoxIncludeFiles.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    : null
            );
        }

        /// <summary>
        /// Initializes the overwrite mode radio buttons to reflect the current overwrite mode setting.
        /// </summary>
        /// <remarks>This method updates the checked state of the overwrite mode radio buttons based on
        /// the value of the overwrite mode configured in the main form. It should be called to synchronize the UI with
        /// the current overwrite mode before displaying or updating the related controls.</remarks>
        private void InitOverrideModeRadio()
        {
            switch (MainForm.overwriteMode)
            {
                case SevenZipHelper.OverwriteMode.OverwriteAll:
                    radioBtnOverwriteAll.Checked = true;
                    break;
                case SevenZipHelper.OverwriteMode.SkipExisting:
                    radioBtnSkipExisting.Checked = true;
                    break;
                case SevenZipHelper.OverwriteMode.RenameNewer:
                    radioBtnRenameNewer.Checked = true;
                    break;
                case SevenZipHelper.OverwriteMode.RenameExisting:
                    radioBtnRenameExisting.Checked = true;
                    break;
            }
        }

        SevenZipHelper.OverwriteMode GetoverwriteModeRadio()
        {
            if (radioBtnSkipExisting.Checked) return SevenZipHelper.OverwriteMode.SkipExisting;
            if (radioBtnRenameNewer.Checked) return SevenZipHelper.OverwriteMode.RenameNewer;
            if (radioBtnRenameExisting.Checked) return SevenZipHelper.OverwriteMode.RenameExisting;
            return SevenZipHelper.OverwriteMode.OverwriteAll; // 默认
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // 触发取消信号
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                labelStatus.Text = "正在终止解压...";
                buttonCancel.Enabled = false; // 防止重复点击
            }
        }

        private async void buttonDownload7Zip_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            var cancellationToken = _cts.Token;
            progressBarCompression.Value = 0;
            buttonDownload7Zip.Enabled = false;
            buttonCancel.Enabled = true;
            buttonStart.Enabled = false;
            labelStatus.Text = "准备开始下载 7-Zip...";
            labelPercent.Text = "0%";
            var progress = new Progress<int>(p =>
            {
                progressBarCompression.Value = Math.Min(p, 100);
                labelPercent.Text = $"{p}%";
                if (progressBarCompression.Value == 100)
                {
                    labelStatus.Text = "7-Zip 下载完成。";
                }
                if (progressBarCompression.Value <= 23)
                {
                    labelStatus.Text = "正在下载 7z.exe...";
                }
                if (progressBarCompression.Value > 23 && progressBarCompression.Value < 100)
                {
                    labelStatus.Text = "正在下载 7z.dll...";
                }
            });
            try
            {
                await Download7ZipExe(cancellationToken, progress);
                Download7ZipCompleted?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("下载 7-Zip 失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "下载 7-Zip 失败。";
                Download7ZipCompleted?.Invoke(this, false);
            }
            finally
            {
                _cts.Dispose();
                buttonDownload7Zip.Enabled = true;
                buttonCancel.Enabled = false;
                buttonStart.Enabled = true;
            }
        }
    }
}
