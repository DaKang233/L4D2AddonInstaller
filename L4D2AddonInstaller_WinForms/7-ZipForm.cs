using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L4D2AddonInstaller_WinForms
{
    public partial class SevenZipForm : Form
    {
        private MainForm mainForm;
        private CancellationTokenSource _cts;
        public SevenZipForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            textBox7ZipPath.Text = MainForm.SevenZipPath;
            textBoxOutputDir.Text = MainForm.OutputDirPath;
            textBoxArchivePath.Text = MainForm.ArchivePath;
            InitOverrideModeRadio();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            MainForm.overwriteMode = GetoverwriteModeRadio();
            MainForm.ArchivePath = textBoxArchivePath.Text;
            MainForm.OutputDirPath = textBoxOutputDir.Text;
            MainForm.SevenZipPath = textBox7ZipPath.Text;
            base.OnFormClosing(e);
        }

        private void buttonArchiveBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择压缩包文件";
                openFileDialog.Filter = "压缩包文件|*.7z;*.zip;*.rar;*.tar;*.gz;*.bz2;*.xz;*.iso;*.cab;*.arj;*.lzh;*.z|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxArchivePath.Text = openFileDialog.FileName;
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
            textBox7ZipPath.Text = SevenZipHelper.Default7ZipFullPath();
            if (string.IsNullOrEmpty(textBox7ZipPath.Text))
            {
                MessageBox.Show("未找到 7z.exe 文件。请手动选择。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void buttonOutputDirDetect_Click(object sender, EventArgs e)
        {
            if ( mainForm == null )
            {
                MessageBox.Show("主窗口引用无效（未加载），无法检测输出目录。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TextBox gamePathTextBox = Universal.FindControlRecursive(mainForm, "textBox1GamePath") as TextBox;
            if (gamePathTextBox == null)
            {
                MessageBox.Show("未找到主窗口中的游戏路径文本框，无法检测输出目录。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string gamePath = gamePathTextBox.Text;
            if (string.IsNullOrEmpty(gamePath) || !System.IO.Directory.Exists(gamePath))
            {
                MessageBox.Show("游戏路径无效，无法检测输出目录。请先在主窗口选择正确的 Left 4 Dead 2 安装目录。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            // 前置检查
            if (string.IsNullOrEmpty(textBox7ZipPath.Text) || !System.IO.File.Exists(textBox7ZipPath.Text)) {
                MessageBox.Show("请指定有效的 7z.exe 的路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBoxArchivePath.Text) || !System.IO.File.Exists(textBoxArchivePath.Text)) {
                MessageBox.Show("请指定有效的压缩包文件路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBoxOutputDir.Text) || !System.IO.Directory.Exists(textBoxOutputDir.Text)) {
                MessageBox.Show("请指定有效的输出目录路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 本地函数：恢复 UI 状态
            void RecoverUIAfterOperation(string StatusText)
            {
                if (!string.IsNullOrEmpty(StatusText))
                    labelStatus.Text = StatusText;
                buttonCancel.Enabled = false;
                buttonStart.Enabled = true;
                _cts?.Dispose();
            }

            // 开始解压：UI 状态更新，进度条绑定
            labelPercent.Text = "0%";
            var overwriteMode = GetoverwriteModeRadio();
            var progress = new Progress<int>(value =>
            {
                progressBarCompression.Value = value;
                labelPercent.Text = $"{value}%";
            });
            labelStatus.Text = $"正在解压文件：{Path.GetFileName(textBoxArchivePath.Text)}...";
            buttonCancel.Enabled = true;
            buttonStart.Enabled = false;
            _cts = new CancellationTokenSource();
            var cancellationToken = _cts.Token;
            bool IsEncrypted = false;

            // 执行解压操作
            if (string.IsNullOrEmpty(textBoxPassword.Text))
            {
                try
                {
                    // 首先验证压缩包是否需要密码
                    IsEncrypted = await SevenZipHelper.IsArchiveEncryptedAsync(
                        textBoxArchivePath.Text,
                        textBox7ZipPath.Text,
                        cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("加密检验操作已被取消。", "已取消", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RecoverUIAfterOperation("解压已取消。");
                    return;
                }
                if (IsEncrypted)
                {
                    MessageBox.Show("压缩包已加密。请在密码框中输入正确的密码后重试。", "需要密码", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    RecoverUIAfterOperation("解压失败，压缩包已加密。");
                    return;
                }
            }
            try
            {
                await SevenZipHelper.ValidateArchiveAsync(
                    textBoxArchivePath.Text,
                    textBox7ZipPath.Text,
                    textBoxPassword.Text ?? null,
                    cancellationToken
                );
            }
            catch (ArchiveRequiresPasswordException)
            {
                MessageBox.Show("该压缩包需要密码才能解压。请在密码框中输入正确的密码后重试。", "需要密码", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RecoverUIAfterOperation("解压失败，压缩包需要密码。");
                return;
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("解压操作已被取消。", "已取消", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RecoverUIAfterOperation("解压已取消。");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("验证压缩包时发生错误：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RecoverUIAfterOperation("解压失败，验证压缩包时发生错误。");
                return;
            }

            try {
                await SevenZipHelper.ExtractAsync(
                    textBoxArchivePath.Text,
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
                labelStatus.Text = "解压完成，文件已解压到指定目录。";
            }
            catch (OperationCanceledException) {
                MessageBox.Show("解压操作已被取消。", "已取消", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RecoverUIAfterOperation("解压已取消。");
                return;
            }
            catch (Exception ex) {
                MessageBox.Show("解压过程中发生错误：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RecoverUIAfterOperation("解压失败，发生错误。");
                return;
            }
            finally {
                RecoverUIAfterOperation("");
            }
        }

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
    }
}
