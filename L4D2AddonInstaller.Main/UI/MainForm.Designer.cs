namespace L4D2AddonInstaller
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnBrowseL4d2Path = new System.Windows.Forms.Button();
            this.textBox1GamePath = new System.Windows.Forms.TextBox();
            this.labelGamePath = new System.Windows.Forms.Label();
            this.btnGamePath = new System.Windows.Forms.Button();
            this.btnSteamPath = new System.Windows.Forms.Button();
            this.labelSteamPath = new System.Windows.Forms.Label();
            this.textBox2SteamPath = new System.Windows.Forms.TextBox();
            this.btnBrowseSteamPath = new System.Windows.Forms.Button();
            this.labelCodeName = new System.Windows.Forms.Label();
            this.textBox3CodeName = new System.Windows.Forms.TextBox();
            this.btnCodeName = new System.Windows.Forms.Button();
            this.btnDetectAllPath = new System.Windows.Forms.Button();
            this.pbDownloadProgress = new System.Windows.Forms.ProgressBar();
            this.lblServerInfo = new System.Windows.Forms.Label();
            this.lblDownloadStatus = new System.Windows.Forms.Label();
            this.lblConsoleCmd = new System.Windows.Forms.Label();
            this.chkAutoStartGame = new System.Windows.Forms.CheckBox();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.groupBoxServerInfo = new System.Windows.Forms.GroupBox();
            this.textBoxConsoleCmd = new System.Windows.Forms.TextBox();
            this.textBoxServerInfo = new System.Windows.Forms.TextBox();
            this.btnCheckForServerInfo = new System.Windows.Forms.Button();
            this.btnStartSteam = new System.Windows.Forms.Button();
            this.lblDownloadStatusTxt = new System.Windows.Forms.Label();
            this.groupBoxPathDetection = new System.Windows.Forms.GroupBox();
            this.groupBoxDownload = new System.Windows.Forms.GroupBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelDownloadPercent = new System.Windows.Forms.Label();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnOneClickFinishAll = new System.Windows.Forms.Button();
            this.btn7ZipForm = new System.Windows.Forms.Button();
            this.btnOpenArchiveDownloadFolder = new System.Windows.Forms.Button();
            this.groupBoxServerInfo.SuspendLayout();
            this.groupBoxPathDetection.SuspendLayout();
            this.groupBoxDownload.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowseL4d2Path
            // 
            this.btnBrowseL4d2Path.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowseL4d2Path.Location = new System.Drawing.Point(587, 84);
            this.btnBrowseL4d2Path.Name = "btnBrowseL4d2Path";
            this.btnBrowseL4d2Path.Size = new System.Drawing.Size(113, 33);
            this.btnBrowseL4d2Path.TabIndex = 10;
            this.btnBrowseL4d2Path.Text = "浏览文件夹...";
            this.btnBrowseL4d2Path.UseVisualStyleBackColor = true;
            this.btnBrowseL4d2Path.Click += new System.EventHandler(this.btnL4d2PathBrowse_Click);
            // 
            // textBox1GamePath
            // 
            this.textBox1GamePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1GamePath.Location = new System.Drawing.Point(10, 120);
            this.textBox1GamePath.Name = "textBox1GamePath";
            this.textBox1GamePath.Size = new System.Drawing.Size(690, 23);
            this.textBox1GamePath.TabIndex = 12;
            // 
            // labelGamePath
            // 
            this.labelGamePath.AutoSize = true;
            this.labelGamePath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelGamePath.Location = new System.Drawing.Point(6, 97);
            this.labelGamePath.Name = "labelGamePath";
            this.labelGamePath.Size = new System.Drawing.Size(65, 20);
            this.labelGamePath.TabIndex = 11;
            this.labelGamePath.Text = "游戏路径";
            // 
            // btnGamePath
            // 
            this.btnGamePath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGamePath.Location = new System.Drawing.Point(468, 84);
            this.btnGamePath.Name = "btnGamePath";
            this.btnGamePath.Size = new System.Drawing.Size(113, 33);
            this.btnGamePath.TabIndex = 9;
            this.btnGamePath.Text = "检索...";
            this.btnGamePath.UseVisualStyleBackColor = true;
            this.btnGamePath.Click += new System.EventHandler(this.btnGamePath_Click);
            // 
            // btnSteamPath
            // 
            this.btnSteamPath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSteamPath.Location = new System.Drawing.Point(468, 20);
            this.btnSteamPath.Name = "btnSteamPath";
            this.btnSteamPath.Size = new System.Drawing.Size(113, 33);
            this.btnSteamPath.TabIndex = 5;
            this.btnSteamPath.Text = "检索...";
            this.btnSteamPath.UseVisualStyleBackColor = true;
            this.btnSteamPath.Click += new System.EventHandler(this.btnSteamPath_Click);
            // 
            // labelSteamPath
            // 
            this.labelSteamPath.AutoSize = true;
            this.labelSteamPath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelSteamPath.Location = new System.Drawing.Point(6, 33);
            this.labelSteamPath.Name = "labelSteamPath";
            this.labelSteamPath.Size = new System.Drawing.Size(83, 20);
            this.labelSteamPath.TabIndex = 7;
            this.labelSteamPath.Text = "Steam 路径";
            // 
            // textBox2SteamPath
            // 
            this.textBox2SteamPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2SteamPath.Location = new System.Drawing.Point(10, 56);
            this.textBox2SteamPath.Name = "textBox2SteamPath";
            this.textBox2SteamPath.Size = new System.Drawing.Size(690, 23);
            this.textBox2SteamPath.TabIndex = 8;
            // 
            // btnBrowseSteamPath
            // 
            this.btnBrowseSteamPath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowseSteamPath.Location = new System.Drawing.Point(587, 20);
            this.btnBrowseSteamPath.Name = "btnBrowseSteamPath";
            this.btnBrowseSteamPath.Size = new System.Drawing.Size(113, 33);
            this.btnBrowseSteamPath.TabIndex = 6;
            this.btnBrowseSteamPath.Text = "浏览文件夹...";
            this.btnBrowseSteamPath.UseVisualStyleBackColor = true;
            this.btnBrowseSteamPath.Click += new System.EventHandler(this.btnSteamPathBrowse_Click);
            // 
            // labelCodeName
            // 
            this.labelCodeName.AutoSize = true;
            this.labelCodeName.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCodeName.Location = new System.Drawing.Point(6, 30);
            this.labelCodeName.Name = "labelCodeName";
            this.labelCodeName.Size = new System.Drawing.Size(121, 20);
            this.labelCodeName.TabIndex = 14;
            this.labelCodeName.Text = "输入提供的代码号";
            // 
            // textBox3CodeName
            // 
            this.textBox3CodeName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox3CodeName.Location = new System.Drawing.Point(10, 53);
            this.textBox3CodeName.Name = "textBox3CodeName";
            this.textBox3CodeName.Size = new System.Drawing.Size(690, 23);
            this.textBox3CodeName.TabIndex = 15;
            // 
            // btnCodeName
            // 
            this.btnCodeName.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCodeName.Location = new System.Drawing.Point(587, 17);
            this.btnCodeName.Name = "btnCodeName";
            this.btnCodeName.Size = new System.Drawing.Size(113, 33);
            this.btnCodeName.TabIndex = 16;
            this.btnCodeName.Text = "检索...";
            this.btnCodeName.UseVisualStyleBackColor = true;
            this.btnCodeName.Click += new System.EventHandler(this.btnCodeName_Click);
            // 
            // btnDetectAllPath
            // 
            this.btnDetectAllPath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDetectAllPath.Location = new System.Drawing.Point(265, 20);
            this.btnDetectAllPath.Name = "btnDetectAllPath";
            this.btnDetectAllPath.Size = new System.Drawing.Size(198, 33);
            this.btnDetectAllPath.TabIndex = 4;
            this.btnDetectAllPath.Text = "一键同时检索两个路径！";
            this.btnDetectAllPath.UseVisualStyleBackColor = true;
            this.btnDetectAllPath.Click += new System.EventHandler(this.btnDetectAllPath_Click);
            // 
            // pbDownloadProgress
            // 
            this.pbDownloadProgress.Location = new System.Drawing.Point(10, 101);
            this.pbDownloadProgress.Name = "pbDownloadProgress";
            this.pbDownloadProgress.Size = new System.Drawing.Size(690, 21);
            this.pbDownloadProgress.TabIndex = 19;
            // 
            // lblServerInfo
            // 
            this.lblServerInfo.AutoSize = true;
            this.lblServerInfo.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblServerInfo.Location = new System.Drawing.Point(6, 30);
            this.lblServerInfo.Name = "lblServerInfo";
            this.lblServerInfo.Size = new System.Drawing.Size(79, 20);
            this.lblServerInfo.TabIndex = 21;
            this.lblServerInfo.Text = "服务器信息";
            // 
            // lblDownloadStatus
            // 
            this.lblDownloadStatus.AutoSize = true;
            this.lblDownloadStatus.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDownloadStatus.Location = new System.Drawing.Point(95, 79);
            this.lblDownloadStatus.Name = "lblDownloadStatus";
            this.lblDownloadStatus.Size = new System.Drawing.Size(18, 20);
            this.lblDownloadStatus.TabIndex = 18;
            this.lblDownloadStatus.Text = "...";
            // 
            // lblConsoleCmd
            // 
            this.lblConsoleCmd.AutoSize = true;
            this.lblConsoleCmd.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblConsoleCmd.Location = new System.Drawing.Point(6, 61);
            this.lblConsoleCmd.Name = "lblConsoleCmd";
            this.lblConsoleCmd.Size = new System.Drawing.Size(79, 20);
            this.lblConsoleCmd.TabIndex = 23;
            this.lblConsoleCmd.Text = "控制台命令";
            // 
            // chkAutoStartGame
            // 
            this.chkAutoStartGame.AutoSize = true;
            this.chkAutoStartGame.Checked = true;
            this.chkAutoStartGame.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoStartGame.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkAutoStartGame.Location = new System.Drawing.Point(483, 367);
            this.chkAutoStartGame.Name = "chkAutoStartGame";
            this.chkAutoStartGame.Size = new System.Drawing.Size(112, 24);
            this.chkAutoStartGame.TabIndex = 26;
            this.chkAutoStartGame.Text = "自动启动游戏";
            this.chkAutoStartGame.UseVisualStyleBackColor = true;
            // 
            // btnStartGame
            // 
            this.btnStartGame.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStartGame.Location = new System.Drawing.Point(601, 403);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(113, 33);
            this.btnStartGame.TabIndex = 28;
            this.btnStartGame.Text = "启动游戏...";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // groupBoxServerInfo
            // 
            this.groupBoxServerInfo.Controls.Add(this.textBoxConsoleCmd);
            this.groupBoxServerInfo.Controls.Add(this.textBoxServerInfo);
            this.groupBoxServerInfo.Controls.Add(this.lblConsoleCmd);
            this.groupBoxServerInfo.Controls.Add(this.lblServerInfo);
            this.groupBoxServerInfo.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBoxServerInfo.Location = new System.Drawing.Point(12, 345);
            this.groupBoxServerInfo.Name = "groupBoxServerInfo";
            this.groupBoxServerInfo.Size = new System.Drawing.Size(463, 94);
            this.groupBoxServerInfo.TabIndex = 20;
            this.groupBoxServerInfo.TabStop = false;
            this.groupBoxServerInfo.Text = "信息";
            // 
            // textBoxConsoleCmd
            // 
            this.textBoxConsoleCmd.Location = new System.Drawing.Point(90, 61);
            this.textBoxConsoleCmd.Name = "textBoxConsoleCmd";
            this.textBoxConsoleCmd.Size = new System.Drawing.Size(359, 26);
            this.textBoxConsoleCmd.TabIndex = 24;
            // 
            // textBoxServerInfo
            // 
            this.textBoxServerInfo.Location = new System.Drawing.Point(90, 29);
            this.textBoxServerInfo.Name = "textBoxServerInfo";
            this.textBoxServerInfo.Size = new System.Drawing.Size(359, 26);
            this.textBoxServerInfo.TabIndex = 22;
            // 
            // btnCheckForServerInfo
            // 
            this.btnCheckForServerInfo.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCheckForServerInfo.Location = new System.Drawing.Point(481, 403);
            this.btnCheckForServerInfo.Name = "btnCheckForServerInfo";
            this.btnCheckForServerInfo.Size = new System.Drawing.Size(116, 33);
            this.btnCheckForServerInfo.TabIndex = 25;
            this.btnCheckForServerInfo.Text = "检索服务器信息";
            this.btnCheckForServerInfo.UseVisualStyleBackColor = true;
            this.btnCheckForServerInfo.Click += new System.EventHandler(this.btnCheckForServerInfo_Click);
            // 
            // btnStartSteam
            // 
            this.btnStartSteam.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStartSteam.Location = new System.Drawing.Point(601, 362);
            this.btnStartSteam.Name = "btnStartSteam";
            this.btnStartSteam.Size = new System.Drawing.Size(113, 33);
            this.btnStartSteam.TabIndex = 27;
            this.btnStartSteam.Text = "启动 Steam...";
            this.btnStartSteam.UseVisualStyleBackColor = true;
            this.btnStartSteam.Click += new System.EventHandler(this.btnStartSteam_Click);
            // 
            // lblDownloadStatusTxt
            // 
            this.lblDownloadStatusTxt.AutoSize = true;
            this.lblDownloadStatusTxt.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblDownloadStatusTxt.Location = new System.Drawing.Point(6, 79);
            this.lblDownloadStatusTxt.Name = "lblDownloadStatusTxt";
            this.lblDownloadStatusTxt.Size = new System.Drawing.Size(79, 20);
            this.lblDownloadStatusTxt.TabIndex = 17;
            this.lblDownloadStatusTxt.Text = "下载状态：";
            // 
            // groupBoxPathDetection
            // 
            this.groupBoxPathDetection.Controls.Add(this.btnSteamPath);
            this.groupBoxPathDetection.Controls.Add(this.labelSteamPath);
            this.groupBoxPathDetection.Controls.Add(this.textBox2SteamPath);
            this.groupBoxPathDetection.Controls.Add(this.btnBrowseSteamPath);
            this.groupBoxPathDetection.Controls.Add(this.btnGamePath);
            this.groupBoxPathDetection.Controls.Add(this.labelGamePath);
            this.groupBoxPathDetection.Controls.Add(this.textBox1GamePath);
            this.groupBoxPathDetection.Controls.Add(this.btnBrowseL4d2Path);
            this.groupBoxPathDetection.Controls.Add(this.btnDetectAllPath);
            this.groupBoxPathDetection.Location = new System.Drawing.Point(12, 51);
            this.groupBoxPathDetection.Name = "groupBoxPathDetection";
            this.groupBoxPathDetection.Size = new System.Drawing.Size(706, 153);
            this.groupBoxPathDetection.TabIndex = 3;
            this.groupBoxPathDetection.TabStop = false;
            this.groupBoxPathDetection.Text = "路径检测";
            // 
            // groupBoxDownload
            // 
            this.groupBoxDownload.Controls.Add(this.btnOpenArchiveDownloadFolder);
            this.groupBoxDownload.Controls.Add(this.buttonCancel);
            this.groupBoxDownload.Controls.Add(this.labelDownloadPercent);
            this.groupBoxDownload.Controls.Add(this.labelCodeName);
            this.groupBoxDownload.Controls.Add(this.textBox3CodeName);
            this.groupBoxDownload.Controls.Add(this.btnCodeName);
            this.groupBoxDownload.Controls.Add(this.lblDownloadStatusTxt);
            this.groupBoxDownload.Controls.Add(this.lblDownloadStatus);
            this.groupBoxDownload.Controls.Add(this.pbDownloadProgress);
            this.groupBoxDownload.Location = new System.Drawing.Point(12, 207);
            this.groupBoxDownload.Name = "groupBoxDownload";
            this.groupBoxDownload.Size = new System.Drawing.Size(706, 135);
            this.groupBoxDownload.TabIndex = 13;
            this.groupBoxDownload.TabStop = false;
            this.groupBoxDownload.Text = "下载";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCancel.Location = new System.Drawing.Point(468, 17);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(113, 33);
            this.buttonCancel.TabIndex = 29;
            this.buttonCancel.Text = "终止当前操作";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelDownloadPercent
            // 
            this.labelDownloadPercent.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelDownloadPercent.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDownloadPercent.Location = new System.Drawing.Point(593, 79);
            this.labelDownloadPercent.Name = "labelDownloadPercent";
            this.labelDownloadPercent.Size = new System.Drawing.Size(112, 20);
            this.labelDownloadPercent.TabIndex = 20;
            this.labelDownloadPercent.Text = "0%";
            this.labelDownloadPercent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnAbout
            // 
            this.btnAbout.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAbout.Location = new System.Drawing.Point(605, 12);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(113, 33);
            this.btnAbout.TabIndex = 2;
            this.btnAbout.Text = "关于...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnOneClickFinishAll
            // 
            this.btnOneClickFinishAll.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOneClickFinishAll.Location = new System.Drawing.Point(12, 12);
            this.btnOneClickFinishAll.Name = "btnOneClickFinishAll";
            this.btnOneClickFinishAll.Size = new System.Drawing.Size(210, 33);
            this.btnOneClickFinishAll.TabIndex = 1;
            this.btnOneClickFinishAll.Text = "一键傻瓜式完成检索和下载！";
            this.btnOneClickFinishAll.UseVisualStyleBackColor = true;
            this.btnOneClickFinishAll.Click += new System.EventHandler(this.btnOneClickFinishAll_Click);
            // 
            // btn7ZipForm
            // 
            this.btn7ZipForm.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn7ZipForm.Location = new System.Drawing.Point(486, 12);
            this.btn7ZipForm.Name = "btn7ZipForm";
            this.btn7ZipForm.Size = new System.Drawing.Size(113, 33);
            this.btn7ZipForm.TabIndex = 13;
            this.btn7ZipForm.Text = "解压缩功能";
            this.btn7ZipForm.UseVisualStyleBackColor = true;
            this.btn7ZipForm.Click += new System.EventHandler(this.btn7ZipForm_Click);
            // 
            // btnOpenArchiveDownloadFolder
            // 
            this.btnOpenArchiveDownloadFolder.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOpenArchiveDownloadFolder.Location = new System.Drawing.Point(300, 17);
            this.btnOpenArchiveDownloadFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOpenArchiveDownloadFolder.Name = "btnOpenArchiveDownloadFolder";
            this.btnOpenArchiveDownloadFolder.Size = new System.Drawing.Size(163, 33);
            this.btnOpenArchiveDownloadFolder.TabIndex = 30;
            this.btnOpenArchiveDownloadFolder.Text = "打开压缩包下载目录";
            this.btnOpenArchiveDownloadFolder.UseVisualStyleBackColor = true;
            this.btnOpenArchiveDownloadFolder.Click += new System.EventHandler(this.btnOpenArchiveDownloadFolder_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(729, 448);
            this.Controls.Add(this.btn7ZipForm);
            this.Controls.Add(this.btnOneClickFinishAll);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.groupBoxDownload);
            this.Controls.Add(this.groupBoxPathDetection);
            this.Controls.Add(this.btnStartSteam);
            this.Controls.Add(this.btnCheckForServerInfo);
            this.Controls.Add(this.groupBoxServerInfo);
            this.Controls.Add(this.btnStartGame);
            this.Controls.Add(this.chkAutoStartGame);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "《求生之路 2》附加组件安装器";
            this.groupBoxServerInfo.ResumeLayout(false);
            this.groupBoxServerInfo.PerformLayout();
            this.groupBoxPathDetection.ResumeLayout(false);
            this.groupBoxPathDetection.PerformLayout();
            this.groupBoxDownload.ResumeLayout(false);
            this.groupBoxDownload.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnBrowseL4d2Path;
        private System.Windows.Forms.Label labelGamePath;
        private System.Windows.Forms.Button btnGamePath;
        private System.Windows.Forms.Button btnSteamPath;
        private System.Windows.Forms.Label labelSteamPath;
        private System.Windows.Forms.TextBox textBox2SteamPath;
        private System.Windows.Forms.Button btnBrowseSteamPath;
        private System.Windows.Forms.Label labelCodeName;
        private System.Windows.Forms.TextBox textBox3CodeName;
        private System.Windows.Forms.Button btnCodeName;
        private System.Windows.Forms.Button btnDetectAllPath;
        private System.Windows.Forms.ProgressBar pbDownloadProgress;
        private System.Windows.Forms.Label lblServerInfo;
        private System.Windows.Forms.Label lblDownloadStatus;
        private System.Windows.Forms.Label lblConsoleCmd;
        private System.Windows.Forms.CheckBox chkAutoStartGame;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.GroupBox groupBoxServerInfo;
        private System.Windows.Forms.TextBox textBoxConsoleCmd;
        private System.Windows.Forms.TextBox textBoxServerInfo;
        private System.Windows.Forms.Button btnCheckForServerInfo;
        private System.Windows.Forms.Button btnStartSteam;
        private System.Windows.Forms.Label lblDownloadStatusTxt;
        private System.Windows.Forms.GroupBox groupBoxPathDetection;
        private System.Windows.Forms.GroupBox groupBoxDownload;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnOneClickFinishAll;
        private System.Windows.Forms.Button btn7ZipForm;
        private System.Windows.Forms.Label labelDownloadPercent;
        public System.Windows.Forms.TextBox textBox1GamePath;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button btnOpenArchiveDownloadFolder;
    }
}

