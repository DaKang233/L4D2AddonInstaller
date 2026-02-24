using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using L4D2AddonInstaller.Parser;
using L4D2AddonInstaller.Helper;
using L4D2AddonInstaller.Services;

namespace L4D2AddonInstaller
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            textBoxServerInfo.ReadOnly = true;
            textBoxConsoleCmd.ReadOnly = true;
            textBoxServerInfo.BackColor = SystemColors.Control; // 禁用编辑背景色
            textBoxConsoleCmd.BackColor = SystemColors.Control;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 禁止调整窗口大小
            string appVersion = Application.ProductVersion;
            string appTitle = "《求生之路 2》附加组件安装器 v" + appVersion;
            this.Text = appTitle;
        }
        // 全局变量
        public static SevenZipHelper.OverwriteMode overwriteMode = SevenZipHelper.OverwriteMode.OverwriteAll;
        public static bool IsOneClickAction = false;
        public event EventHandler<bool> InstallationFinished;
        public CancellationTokenSource _cts;
        private readonly IInstallService _installService = new InstallService();

        // 解压相关全局变量（关闭窗口时保留值）
        public static string ArchivePath = "";
        public static string OutputDirPath = "";
        public static string SevenZipPath = "";
        public static string GamePath = "";
        /// <summary>
        /// Represents a request to extract files from one or more 7-Zip archives.
        /// </summary>
        /// <remarks>Use this class to specify the parameters required for extracting files from 7-Zip
        /// archives, including the archive paths, output directory, and extraction options. All properties must be set
        /// before initiating the extraction process.</remarks>
        public class ExtractRequest
        {
            /// <summary>
            /// 7-Zip压缩包路径（多个用分号分隔）
            /// </summary>
            public string ArchivePath { get; set; }
            /// <summary>
            /// 解压输出目录路径
            /// </summary>
            public string OutputDirPath { get; set; }
            /// <summary>
            /// 7-Zip可执行文件路径
            /// </summary>
            public string SevenZipPath { get; set; }
            /// <summary>
            /// 是否自动解压
            /// </summary>
            public bool IsAutoExtract { get; set; }

            public string IncludeFiles { get; set; }
        }

        /// <summary>
        /// Determines whether the current Steam path is valid by checking for the presence of the Steam executable.
        /// </summary>
        /// <returns>true if the specified Steam path exists and contains the 'steam.exe' file; otherwise, false.</returns>
        bool IsSteamPathValid()
        {
            var steamPath = textBox2SteamPath.Text.Trim();
            if (string.IsNullOrEmpty(steamPath) || !Directory.Exists(steamPath))
                return false;
            var steamExePath = Path.Combine(steamPath, "steam.exe");
            return File.Exists(steamExePath);
        }

        bool IsL4D2PathValid()
        {
            var gamePath = textBox1GamePath.Text.Trim();
            if (string.IsNullOrEmpty(gamePath) || !Directory.Exists(gamePath))
                return false;
            var l4d2ExePath = Path.Combine(gamePath, "left4dead2.exe");
            return File.Exists(l4d2ExePath);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 让用户确认是否要退出程序
            DialogResult result = MessageBox.Show("是否要退出程序？", "提示",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
                base.OnFormClosing(e);
            else { this.Show(); e.Cancel = true; }
            ; // 取消关闭，继续显示窗口
        }

        /// <summary>
        /// 处理L4D2路径浏览按钮的Click事件，允许用户选择《求生之路2》（Left 4 Dead 2）的安装目录。
        /// </summary>
        /// <remarks>当用户选择文件夹并确认对话框后，所选路径会被赋值到游戏路径文本框中。
        /// 该对话框禁止创建新文件夹。</remarks>
        /// <param name="sender">事件源（通常为浏览按钮控件）。</param>
        /// <param name="e">包含事件数据的EventArgs对象。</param>
        private void btnL4d2PathBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog l4d2FolderDialog = new FolderBrowserDialog())
            {
                l4d2FolderDialog.Description = "选择《求生之路 2》的安装文件夹（例如：C:\\Program Files (x86)\\Steam\\steamapps\\common\\Left 4 Dead 2）";
                l4d2FolderDialog.ShowNewFolderButton = false;
                if (l4d2FolderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1GamePath.Text = l4d2FolderDialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 处理Steam路径浏览按钮的Click事件，允许用户选择Steam的安装目录。
        /// </summary>
        /// <remarks>打开文件夹浏览对话框供用户选择Steam目录。
        /// 若用户确认选择，所选路径会被赋值到Steam路径文本框中。</remarks>
        /// <param name="sender">事件源（通常为浏览按钮控件）。</param>
        /// <param name="e">包含事件数据的EventArgs对象。</param>
        private void btnSteamPathBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog steamFolderDialog = new FolderBrowserDialog())
            {
                steamFolderDialog.Description = "选择 Steam 安装文件夹（例如：C:\\Program Files (x86)\\Steam）";
                steamFolderDialog.ShowNewFolderButton = false;
                if (steamFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox2SteamPath.Text = steamFolderDialog.SelectedPath;
                }
            }
        }

        // 一键检索Steam和L4D2路径
        /// <summary>
        /// 处理“一键检索！”按钮的点击事件，自动检测并设置 Steam 及《求生之路2》（Left 4 Dead 2）的安装路径。
        /// </summary>
        /// <remarks>若无法检测到 Steam 安装路径，将显示提示信息告知用户。
        /// 检测过程中该按钮会被临时禁用，以防止重复点击。</remarks>
        /// <param name="sender">事件源（通常为“检测所有路径”按钮）。</param>
        /// <param name="e">包含事件数据的 EventArgs 实例。</param>
        private void btnDetectAllPath_Click(object sender, EventArgs e)
        {
            btnDetectAllPath.Enabled = false;
            try
            {
                btnSteamPath_Click(sender, e);
                if (!string.IsNullOrEmpty(textBox2SteamPath.Text.Trim()))
                    btnGamePath_Click(sender, e);
                else
                {
                    MessageBox.Show("未检索到 Steam 安装路径。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnDetectAllPath.Enabled = true;
            }
        }

        // 检索Steam路径
        /// <summary>
        /// Retrieves the installation path of the Steam client from the Windows registry.
        /// </summary>
        /// <remarks>This method checks both the current user and local machine registry hives for the
        /// Steam installation path. The returned path may be normalized to use single backslashes. This method does not
        /// validate whether the path exists on disk.</remarks>
        /// <returns>A string containing the Steam installation path if found; otherwise, null.</returns>
        public string GetSteamPath()
        {
            string steamPath = null;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\\Valve\\Steam"))
            {
                if (key != null)
                {
                    steamPath = key.GetValue("SteamPath")?.ToString();
                }
            }
            if (string.IsNullOrEmpty(steamPath))
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\\Valve\\Steam"))
                {
                    if (key != null)
                    {
                        steamPath = key.GetValue("SteamPath")?.ToString();
                    }
                }
            }
            if (!string.IsNullOrEmpty(steamPath))
            {
                steamPath = steamPath.Replace("\\\\", "\\");
            }

            return steamPath;
        }

        /// <summary>
        /// Handles the Click event for the Steam Path selection button, automatically detecting and displaying the
        /// Steam installation path if found.
        /// </summary>
        /// <remarks>If the Steam installation path cannot be detected or does not exist, a message box is
        /// displayed to inform the user. The detected path is shown in the associated text box if valid.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
        private void btnSteamPath_Click(object sender, EventArgs e)
        {
            string steamPath = GetSteamPath();
            if (string.IsNullOrEmpty(steamPath))
            {
                MessageBox.Show("未找到 Steam 的安装路径，请尝试手动输入/选择", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Directory.Exists(steamPath))
            {
                MessageBox.Show($"Steam 安装路径不存在：\n{steamPath}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBox2SteamPath.Text = Path.GetFullPath(steamPath);
        }

        /// <summary>
        /// Handles the click event for the game path selection button, automatically locates and sets the installation
        /// path of Left 4 Dead 2 based on the configured Steam installation directory.
        /// </summary>
        /// <remarks>If the Steam installation path or required files are missing, or if Left 4 Dead 2 is
        /// not found, an informational or error message is displayed to the user. The method does not throw exceptions
        /// to the caller; all errors are handled and reported via message dialogs.</remarks>
        /// <param name="sender">The source of the event, typically the button control that was clicked.</param>
        /// <param name="e">An EventArgs object containing data related to the click event.</param>
        private void btnGamePath_Click(object sender, EventArgs e)
        {
            try
            {
                string steamInstallPath = textBox2SteamPath.Text.Trim();
                if (string.IsNullOrEmpty(steamInstallPath))
                {
                    MessageBox.Show("请先设置 Steam 的安装路径。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!Directory.Exists(steamInstallPath))
                {
                    MessageBox.Show($"Steam 安装路径不存在：\n{steamInstallPath}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string libraryVdfPath = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(libraryVdfPath))
                {
                    MessageBox.Show($"未找到 Steam 的 libraryfolders.vdf 文件\n路径：{libraryVdfPath}", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string l4d2LibraryPath = Parser.SteamLibraryVdfParser.GetLibraryPathByGameId(libraryVdfPath, "550");
                if (string.IsNullOrEmpty(l4d2LibraryPath))
                {
                    MessageBox.Show("未找到 L4D2 的安装路径，请确认游戏已安装到该 Steam 库。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string l4d2Path = Path.Combine(l4d2LibraryPath, "steamapps", "common", "Left 4 Dead 2");
                if (Directory.Exists(l4d2Path))
                {
                    textBox1GamePath.Text = l4d2Path;
                }
                else
                {
                    MessageBox.Show($"解析到 L4D2 的安装路径但未找到：\n{l4d2Path}", "警告",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1GamePath.Text = l4d2Path;
                    return;
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"文件未找到：{ex.FileName}\n{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"文件读取失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"参数错误：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"检索 L4D2 路径时出错: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下载列表的远程URL地址
        /// </summary>
        /// <summary>
        /// 处理代号下载按钮的点击事件：验证用户输入、获取对应的下载配置，
        /// 并异步下载《求生之路2》（Left 4 Dead 2）所需的插件。
        /// </summary>
        /// <remarks>此方法在启动下载流程前，会对代号和游戏路径执行输入验证。
        /// 它从远程源获取插件配置、构建下载URL，并并发下载插件。
        /// 整个操作过程中会更新UI以反映进度和状态。
        /// 若勾选了“自动启动游戏”选项，游戏会在安装成功后自动启动。
        /// 针对无效输入、配置异常或下载失败等情况，会向用户显示错误提示信息。</remarks>
        /// <param name="sender">事件源（通常为被点击的按钮）。</param>
        /// <param name="e">包含事件数据的 EventArgs 对象。</param>
        private async void btnCodeName_Click(object sender, EventArgs e)
        {
            var code = textBox3CodeName.Text.Trim();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("请输入下载代号（如1、231）", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(textBox1GamePath.Text.Trim()))
            {
                MessageBox.Show("请先检索/选择L4D2安装路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!IsSteamPathValid() || !IsL4D2PathValid())
            {
                MessageBox.Show("Steam或L4D2路径无效，请确认路径正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnCodeName.Enabled = false;
            buttonCancel.Enabled = true;
            pbDownloadProgress.Value = 0;
            lblDownloadStatus.Text = "正在获取下载配置...";
            textBoxServerInfo.Text = "";
            textBoxConsoleCmd.Text = "";
            labelDownloadPercent.Text = "0%";
            InstallationFinished?.Invoke(this, false);

            _cts = new CancellationTokenSource();
            var cancellationToken = _cts.Token;
            bool containsArchive = false;

            try
            {
                var progress = new Progress<InstallProgressInfo>(info =>
                {
                    lblDownloadStatus.Text = info.StatusMessage;
                    pbDownloadProgress.Value = Math.Max(0, Math.Min(100, info.Percent));
                    labelDownloadPercent.Text = $"{pbDownloadProgress.Value}%";

                    if (!string.IsNullOrWhiteSpace(info.ServerDisplay))
                    {
                        textBoxServerInfo.Text = info.ServerDisplay;
                        textBoxConsoleCmd.Text = info.ConnectCommand;
                    }
                });

                var result = await _installService.DownloadAndInstallAsync(code, textBox1GamePath.Text.Trim(), progress, cancellationToken);
                containsArchive = result.ContainsArchive;

                bool extracted = false;
                if (containsArchive)
                {
                    var needExtract = IsOneClickAction || MessageBox.Show("检测到压缩包，请确认是否需要解压？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK;
                    if (needExtract)
                    {
                        extracted = OpenExtractDialog(result.DownloadedArchivePaths);
                    }
                }

                lblDownloadStatus.Text = extracted ? "所有附加组件下载并安装完成！" : result.StatusMessage;

                if (chkAutoStartGame.Checked && !string.IsNullOrEmpty(result.Host) && !string.IsNullOrEmpty(result.Port) && (!containsArchive || extracted))
                {
                    await StartL4D2GameAsync(result.Host, result.Port);
                }
            }
            catch (OperationCanceledException)
            {
                lblDownloadStatus.Text = "下载已取消。";
            }
            catch (Exception ex)
            {
                lblDownloadStatus.Text = "下载/解析失败！";
                MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                InstallationFinished?.Invoke(this, true);
                btnCodeName.Enabled = true;
                buttonCancel.Enabled = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private bool OpenExtractDialog(IReadOnlyList<string> downloadedArchives)
        {
            try
            {
                if (downloadedArchives == null || downloadedArchives.Count == 0)
                    return false;

                var validArchives = downloadedArchives
                    .Where(File.Exists)
                    .ToArray();

                if (!validArchives.Any())
                    return false;

                var request = new ExtractRequest
                {
                    ArchivePath = string.Join(";", validArchives),
                    OutputDirPath = Path.Combine(textBox1GamePath.Text.Trim(), "left4dead2", "addons"),
                    SevenZipPath = SevenZipHelper.Default7ZipFullPath(),
                    IsAutoExtract = true
                };

                bool success = false;
                using (var form = new SevenZipForm(request))
                {
                    form.ExtractionCompleted += (_, ok) => success = ok;
                    form.ShowDialog();
                }

                return success;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解压缩附加组件失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 通过 Steam 协议启动《求生之路2》（Left 4 Dead 2）游戏客户端，以异步方式连接到指定的主机和端口。
        /// </summary>
        /// <remarks>此方法通过协议 URL 启动 Steam 客户端，从而启动《求生之路2》并连接到指定的游戏服务器。
        /// 系统中必须已安装且正确配置 Steam 客户端，该操作才能成功执行。
        /// 若 Steam 未运行，将通过消息框告知用户。</remarks>
        /// <param name="host">要连接的游戏服务器的主机名或 IP 地址。不能为 null 或空值。</param>
        /// <param name="port">游戏服务器的端口号（字符串形式）。若为空值，连接将使用默认端口。</param>
        /// <returns>表示启动游戏客户端异步操作的任务（Task）。</returns>
        private async Task StartL4D2GameAsync(string host, string port)
        {
            if (string.IsNullOrEmpty(textBox1GamePath.Text.Trim()))
            {
                MessageBox.Show("请先检索/选择L4D2安装路径。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!Directory.Exists(textBox1GamePath.Text.Trim()))
            {
                MessageBox.Show("L4D2 安装路径不存在，请确认游戏已安装。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!IsL4D2PathValid() || !IsSteamPathValid())
            {
                MessageBox.Show("L4D2或Steam路径无效，请确认路径正确。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!ProcessHelper.IsProcessRunning("steam"))
            {
                DialogResult result = MessageBox.Show("检测到 Steam 未启动，是否尝试启动 Steam？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        var steamExe = Path.Combine(textBox2SteamPath.Text.Trim(), "steam.exe");
                        ProcessHelper.StartExecutable(steamExe);
                        // 等待 Steam 启动完成
                        lblDownloadStatus.Text = "正在启动 Steam，请稍候...";
                        MessageBox.Show("请在 Steam 客户端中登录并确保 Steam 已完全启动（进入到库界面），点击确定后继续启动游戏。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"启动 Steam 失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else return;
            }
            try
            {
                lblDownloadStatus.Text = "正在启动游戏...";
                // Steam协议：steam://rungameid/550//+connect host:port
                var connectArgs = string.IsNullOrEmpty(port) ? host : $"{host}:{port}";
                var steamUrl = $"steam://rungameid/550//+connect {connectArgs}";
                // 启动Steam协议（异步等待进程启动）
                ProcessHelper.StartUri(steamUrl);
                lblDownloadStatus.Text = "游戏已启动！";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动游戏失败：{ex.Message}\n请尝试手动启动Steam并运行L4D2。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblDownloadStatus.Text = "游戏启动失败！";
            }
        }

        /// <summary>
        /// 手动启动游戏按钮的点击事件处理器：从服务器信息文本框中提取主机和端口，然后通过 Steam 协议启动《求生之路2》游戏客户端。
        /// </summary>
        /// <remarks>此方法首先验证服务器信息文本框中的内容是否有效。如果内容不存在，将显示一个信息框提示用户并终止当前操作。</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnStartGame_Click(object sender, EventArgs e)
        {
            //var host = textBoxServerInfo.Text.Replace("游戏服务器：", "").Split(':').FirstOrDefault();
            //var port = textBoxServerInfo.Text.Replace("游戏服务器：", "").Split(':').LastOrDefault();
            var host = textBoxServerInfo.Text.Split(':').FirstOrDefault().Trim();
            var port = textBoxServerInfo.Text.Contains(":") ?
                textBoxServerInfo.Text.Split(':').LastOrDefault().Trim() : ""; // L4D2 默认 27015 端口可省略
            if (string.IsNullOrEmpty(host))
            {
                MessageBox.Show("请先解析下载代号获取服务器信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            await StartL4D2GameAsync(host, port);
        }

        /// <summary>
        /// 处理点击“获取服务器信息”按钮的事件。如果成功获取到对应代号的服务器信息，则显示在服务器信息文本框中。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnCheckForServerInfo_Click(object sender, EventArgs e)
        {
            try
            {
                var code = textBox3CodeName.Text.Trim();
                if (string.IsNullOrEmpty(code))
                {
                    MessageBox.Show("请输入下载代号（如1、231）", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var info = await _installService.ResolveServerInfoAsync(code, CancellationToken.None);
                textBoxServerInfo.Text = info.ServerDisplay;
                textBoxConsoleCmd.Text = info.ConnectCommand;
                lblDownloadStatus.Text = "服务器信息获取成功";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取服务器信息失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartSteam_Click(object sender, EventArgs e)
        {
            if (!IsSteamPathValid())
            {
                MessageBox.Show("Steam路径无效，请确认Steam已安装且路径正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var steamExe = Path.Combine(textBox2SteamPath.Text.Trim(), "steam.exe");
                ProcessHelper.StartExecutable(steamExe);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动 Steam 失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 关于按钮的点击事件处理器：显示应用程序的关于对话框。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }

        /// <summary>
        /// 一键完成所有操作按钮的点击事件处理器：检索游戏路径、下载插件、启动游戏。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOneClickFinishAll_Click(object sender, EventArgs e)
        {
            btnOneClickFinishAll.Enabled = false;
            try
            {
                if (string.IsNullOrEmpty(textBox3CodeName.Text))
                {
                    MessageBox.Show("请先在下载区域输入下载代号（如1、231）", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnOneClickFinishAll.Enabled = true;
                    return;
                }
                IsOneClickAction = true;
                btnDetectAllPath_Click(sender, e);
                btnCodeName_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"一键完成操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                IsOneClickAction = false;
                btnOneClickFinishAll.Enabled = true;
            }
            finally
            {
                InstallationFinished += (s, success) =>
                {
                    if (success)
                    {
                        IsOneClickAction = false;
                        btnOneClickFinishAll.Enabled = true;
                    }
                };
            }
        }

        /// <summary>
        /// 解压缩工具按钮的点击事件处理器：显示7-Zip管理对话框。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn7ZipForm_Click(object sender, EventArgs e)
        {
            GamePath = textBox1GamePath.Text.Trim();
            using (SevenZipForm sevenZipForm = new SevenZipForm())
            {
                sevenZipForm.ShowDialog();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                buttonCancel.Enabled = false;
                lblDownloadStatus.Text = "正在取消下载，请稍候...";
            }
        }

        private void btnOpenArchiveDownloadFolder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1GamePath.Text.Trim()))
            {
                MessageBox.Show("请先检索/选择L4D2安装路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!IsL4D2PathValid())
            {
                MessageBox.Show("L4D2安装路径似乎不正确，请确认", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "explorer.exe";
                var archiveDownloadPath = Path.Combine(textBox1GamePath.Text.Trim(), "l4d2InstallToolDownloads");
                if (!Directory.Exists(archiveDownloadPath))
                {
                    Directory.CreateDirectory(archiveDownloadPath);
                }
                process.StartInfo.Arguments = archiveDownloadPath;
                process.Start();
            }
        }
    }
}
