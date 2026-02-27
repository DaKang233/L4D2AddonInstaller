namespace L4D2AddonInstaller
{
    partial class SevenZipForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBarCompression = new System.Windows.Forms.ProgressBar();
            this.buttonArchiveBrowse = new System.Windows.Forms.Button();
            this.textBoxArchivePath = new System.Windows.Forms.TextBox();
            this.labelArchivePath = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelStatusText = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelPercent = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label7ZipPath = new System.Windows.Forms.Label();
            this.textBox7ZipPath = new System.Windows.Forms.TextBox();
            this.button7ZipPathBrowse = new System.Windows.Forms.Button();
            this.labelOutputDir = new System.Windows.Forms.Label();
            this.textBoxOutputDir = new System.Windows.Forms.TextBox();
            this.buttonOutputDirBrowse = new System.Windows.Forms.Button();
            this.button7ZipPathDetect = new System.Windows.Forms.Button();
            this.buttonOutputDirDetect = new System.Windows.Forms.Button();
            this.buttonOpenOutputDir = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.groupBoxExtraOptions = new System.Windows.Forms.GroupBox();
            this.labelIncludeFiles = new System.Windows.Forms.Label();
            this.textBoxIncludeFiles = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.radioBtnRenameExisting = new System.Windows.Forms.RadioButton();
            this.radioBtnRenameNewer = new System.Windows.Forms.RadioButton();
            this.radioBtnSkipExisting = new System.Windows.Forms.RadioButton();
            this.radioBtnOverwriteAll = new System.Windows.Forms.RadioButton();
            this.buttonDownload7Zip = new System.Windows.Forms.Button();
            this.groupBoxExtraOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarCompression
            // 
            this.progressBarCompression.Location = new System.Drawing.Point(11, 243);
            this.progressBarCompression.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBarCompression.Name = "progressBarCompression";
            this.progressBarCompression.Size = new System.Drawing.Size(579, 24);
            this.progressBarCompression.TabIndex = 0;
            // 
            // buttonArchiveBrowse
            // 
            this.buttonArchiveBrowse.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonArchiveBrowse.Location = new System.Drawing.Point(502, 69);
            this.buttonArchiveBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonArchiveBrowse.Name = "buttonArchiveBrowse";
            this.buttonArchiveBrowse.Size = new System.Drawing.Size(88, 30);
            this.buttonArchiveBrowse.TabIndex = 1;
            this.buttonArchiveBrowse.Text = "浏览...";
            this.buttonArchiveBrowse.UseVisualStyleBackColor = true;
            this.buttonArchiveBrowse.Click += new System.EventHandler(this.buttonArchiveBrowse_Click);
            // 
            // textBoxArchivePath
            // 
            this.textBoxArchivePath.Location = new System.Drawing.Point(11, 71);
            this.textBoxArchivePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxArchivePath.Name = "textBoxArchivePath";
            this.textBoxArchivePath.Size = new System.Drawing.Size(485, 26);
            this.textBoxArchivePath.TabIndex = 2;
            // 
            // labelArchivePath
            // 
            this.labelArchivePath.AutoSize = true;
            this.labelArchivePath.Location = new System.Drawing.Point(7, 47);
            this.labelArchivePath.Name = "labelArchivePath";
            this.labelArchivePath.Size = new System.Drawing.Size(316, 20);
            this.labelArchivePath.TabIndex = 3;
            this.labelArchivePath.Text = "压缩包路径(支持多个压缩包,使用浏览按钮来多选)";
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonStart.Location = new System.Drawing.Point(11, 13);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(88, 30);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "开始解压";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelStatusText
            // 
            this.labelStatusText.AutoSize = true;
            this.labelStatusText.Location = new System.Drawing.Point(7, 219);
            this.labelStatusText.Name = "labelStatusText";
            this.labelStatusText.Size = new System.Drawing.Size(51, 20);
            this.labelStatusText.TabIndex = 6;
            this.labelStatusText.Text = "状态：";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(64, 219);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(18, 20);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "...";
            // 
            // labelPercent
            // 
            this.labelPercent.Location = new System.Drawing.Point(528, 219);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelPercent.Size = new System.Drawing.Size(62, 20);
            this.labelPercent.TabIndex = 8;
            this.labelPercent.Text = "0%";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCancel.Location = new System.Drawing.Point(199, 13);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(88, 30);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "终止";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label7ZipPath
            // 
            this.label7ZipPath.AutoSize = true;
            this.label7ZipPath.Location = new System.Drawing.Point(7, 105);
            this.label7ZipPath.Name = "label7ZipPath";
            this.label7ZipPath.Size = new System.Drawing.Size(77, 20);
            this.label7ZipPath.TabIndex = 12;
            this.label7ZipPath.Text = "7-Zip 路径";
            // 
            // textBox7ZipPath
            // 
            this.textBox7ZipPath.Location = new System.Drawing.Point(11, 129);
            this.textBox7ZipPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox7ZipPath.Name = "textBox7ZipPath";
            this.textBox7ZipPath.ReadOnly = true;
            this.textBox7ZipPath.Size = new System.Drawing.Size(391, 26);
            this.textBox7ZipPath.TabIndex = 11;
            // 
            // button7ZipPathBrowse
            // 
            this.button7ZipPathBrowse.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7ZipPathBrowse.Location = new System.Drawing.Point(502, 127);
            this.button7ZipPathBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button7ZipPathBrowse.Name = "button7ZipPathBrowse";
            this.button7ZipPathBrowse.Size = new System.Drawing.Size(88, 30);
            this.button7ZipPathBrowse.TabIndex = 10;
            this.button7ZipPathBrowse.Text = "浏览...";
            this.button7ZipPathBrowse.UseVisualStyleBackColor = true;
            this.button7ZipPathBrowse.Click += new System.EventHandler(this.button7ZipPathBrowse_Click);
            // 
            // labelOutputDir
            // 
            this.labelOutputDir.AutoSize = true;
            this.labelOutputDir.Location = new System.Drawing.Point(7, 165);
            this.labelOutputDir.Name = "labelOutputDir";
            this.labelOutputDir.Size = new System.Drawing.Size(65, 20);
            this.labelOutputDir.TabIndex = 15;
            this.labelOutputDir.Text = "输出目录";
            // 
            // textBoxOutputDir
            // 
            this.textBoxOutputDir.Location = new System.Drawing.Point(11, 189);
            this.textBoxOutputDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxOutputDir.Name = "textBoxOutputDir";
            this.textBoxOutputDir.Size = new System.Drawing.Size(391, 26);
            this.textBoxOutputDir.TabIndex = 14;
            // 
            // buttonOutputDirBrowse
            // 
            this.buttonOutputDirBrowse.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonOutputDirBrowse.Location = new System.Drawing.Point(502, 187);
            this.buttonOutputDirBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonOutputDirBrowse.Name = "buttonOutputDirBrowse";
            this.buttonOutputDirBrowse.Size = new System.Drawing.Size(88, 30);
            this.buttonOutputDirBrowse.TabIndex = 13;
            this.buttonOutputDirBrowse.Text = "浏览...";
            this.buttonOutputDirBrowse.UseVisualStyleBackColor = true;
            this.buttonOutputDirBrowse.Click += new System.EventHandler(this.buttonOutputDirBrowse_Click);
            // 
            // button7ZipPathDetect
            // 
            this.button7ZipPathDetect.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7ZipPathDetect.Location = new System.Drawing.Point(408, 127);
            this.button7ZipPathDetect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button7ZipPathDetect.Name = "button7ZipPathDetect";
            this.button7ZipPathDetect.Size = new System.Drawing.Size(88, 30);
            this.button7ZipPathDetect.TabIndex = 16;
            this.button7ZipPathDetect.Text = "检索...";
            this.button7ZipPathDetect.UseVisualStyleBackColor = true;
            this.button7ZipPathDetect.Click += new System.EventHandler(this.button7ZipPathDetect_Click);
            // 
            // buttonOutputDirDetect
            // 
            this.buttonOutputDirDetect.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonOutputDirDetect.Location = new System.Drawing.Point(408, 187);
            this.buttonOutputDirDetect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonOutputDirDetect.Name = "buttonOutputDirDetect";
            this.buttonOutputDirDetect.Size = new System.Drawing.Size(88, 30);
            this.buttonOutputDirDetect.TabIndex = 17;
            this.buttonOutputDirDetect.Text = "检索...";
            this.buttonOutputDirDetect.UseVisualStyleBackColor = true;
            this.buttonOutputDirDetect.Click += new System.EventHandler(this.buttonOutputDirDetect_Click);
            // 
            // buttonOpenOutputDir
            // 
            this.buttonOpenOutputDir.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonOpenOutputDir.Location = new System.Drawing.Point(481, 13);
            this.buttonOpenOutputDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonOpenOutputDir.Name = "buttonOpenOutputDir";
            this.buttonOpenOutputDir.Size = new System.Drawing.Size(109, 30);
            this.buttonOpenOutputDir.TabIndex = 18;
            this.buttonOpenOutputDir.Text = "打开输出目录";
            this.buttonOpenOutputDir.UseVisualStyleBackColor = true;
            this.buttonOpenOutputDir.Click += new System.EventHandler(this.buttonOpenOutputDir_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonPause.Location = new System.Drawing.Point(105, 13);
            this.buttonPause.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(88, 30);
            this.buttonPause.TabIndex = 5;
            this.buttonPause.Text = "暂停";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Visible = false;
            // 
            // groupBoxExtraOptions
            // 
            this.groupBoxExtraOptions.Controls.Add(this.labelIncludeFiles);
            this.groupBoxExtraOptions.Controls.Add(this.textBoxIncludeFiles);
            this.groupBoxExtraOptions.Controls.Add(this.labelPassword);
            this.groupBoxExtraOptions.Controls.Add(this.textBoxPassword);
            this.groupBoxExtraOptions.Controls.Add(this.radioBtnRenameExisting);
            this.groupBoxExtraOptions.Controls.Add(this.radioBtnRenameNewer);
            this.groupBoxExtraOptions.Controls.Add(this.radioBtnSkipExisting);
            this.groupBoxExtraOptions.Controls.Add(this.radioBtnOverwriteAll);
            this.groupBoxExtraOptions.Location = new System.Drawing.Point(11, 274);
            this.groupBoxExtraOptions.Name = "groupBoxExtraOptions";
            this.groupBoxExtraOptions.Size = new System.Drawing.Size(579, 76);
            this.groupBoxExtraOptions.TabIndex = 20;
            this.groupBoxExtraOptions.TabStop = false;
            this.groupBoxExtraOptions.Text = "高级选项";
            // 
            // labelIncludeFiles
            // 
            this.labelIncludeFiles.AutoSize = true;
            this.labelIncludeFiles.Location = new System.Drawing.Point(426, 22);
            this.labelIncludeFiles.Name = "labelIncludeFiles";
            this.labelIncludeFiles.Size = new System.Drawing.Size(117, 20);
            this.labelIncludeFiles.TabIndex = 23;
            this.labelIncludeFiles.Text = "过滤文件(可选)：";
            // 
            // textBoxIncludeFiles
            // 
            this.textBoxIncludeFiles.Location = new System.Drawing.Point(430, 44);
            this.textBoxIncludeFiles.Name = "textBoxIncludeFiles";
            this.textBoxIncludeFiles.Size = new System.Drawing.Size(143, 26);
            this.textBoxIncludeFiles.TabIndex = 22;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(287, 22);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(103, 20);
            this.labelPassword.TabIndex = 21;
            this.labelPassword.Text = "密码(如果有)：";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(291, 44);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(133, 26);
            this.textBoxPassword.TabIndex = 4;
            // 
            // radioBtnRenameExisting
            // 
            this.radioBtnRenameExisting.AutoSize = true;
            this.radioBtnRenameExisting.Location = new System.Drawing.Point(128, 46);
            this.radioBtnRenameExisting.Name = "radioBtnRenameExisting";
            this.radioBtnRenameExisting.Size = new System.Drawing.Size(139, 24);
            this.radioBtnRenameExisting.TabIndex = 3;
            this.radioBtnRenameExisting.TabStop = true;
            this.radioBtnRenameExisting.Text = "重命名已存在文件";
            this.radioBtnRenameExisting.UseVisualStyleBackColor = true;
            // 
            // radioBtnRenameNewer
            // 
            this.radioBtnRenameNewer.AutoSize = true;
            this.radioBtnRenameNewer.Location = new System.Drawing.Point(6, 46);
            this.radioBtnRenameNewer.Name = "radioBtnRenameNewer";
            this.radioBtnRenameNewer.Size = new System.Drawing.Size(111, 24);
            this.radioBtnRenameNewer.TabIndex = 2;
            this.radioBtnRenameNewer.TabStop = true;
            this.radioBtnRenameNewer.Text = "重命名新文件";
            this.radioBtnRenameNewer.UseVisualStyleBackColor = true;
            // 
            // radioBtnSkipExisting
            // 
            this.radioBtnSkipExisting.AutoSize = true;
            this.radioBtnSkipExisting.Location = new System.Drawing.Point(128, 25);
            this.radioBtnSkipExisting.Name = "radioBtnSkipExisting";
            this.radioBtnSkipExisting.Size = new System.Drawing.Size(125, 24);
            this.radioBtnSkipExisting.TabIndex = 1;
            this.radioBtnSkipExisting.TabStop = true;
            this.radioBtnSkipExisting.Text = "跳过已存在文件";
            this.radioBtnSkipExisting.UseVisualStyleBackColor = true;
            // 
            // radioBtnOverwriteAll
            // 
            this.radioBtnOverwriteAll.AutoSize = true;
            this.radioBtnOverwriteAll.Location = new System.Drawing.Point(6, 25);
            this.radioBtnOverwriteAll.Name = "radioBtnOverwriteAll";
            this.radioBtnOverwriteAll.Size = new System.Drawing.Size(83, 24);
            this.radioBtnOverwriteAll.TabIndex = 0;
            this.radioBtnOverwriteAll.TabStop = true;
            this.radioBtnOverwriteAll.Text = "覆盖全部";
            this.radioBtnOverwriteAll.UseVisualStyleBackColor = true;
            // 
            // buttonDownload7Zip
            // 
            this.buttonDownload7Zip.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonDownload7Zip.Location = new System.Drawing.Point(387, 13);
            this.buttonDownload7Zip.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonDownload7Zip.Name = "buttonDownload7Zip";
            this.buttonDownload7Zip.Size = new System.Drawing.Size(88, 30);
            this.buttonDownload7Zip.TabIndex = 21;
            this.buttonDownload7Zip.Text = "下载 7-Zip";
            this.buttonDownload7Zip.UseVisualStyleBackColor = true;
            this.buttonDownload7Zip.Click += new System.EventHandler(this.buttonDownload7Zip_Click);
            // 
            // SevenZipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 362);
            this.Controls.Add(this.buttonDownload7Zip);
            this.Controls.Add(this.groupBoxExtraOptions);
            this.Controls.Add(this.buttonOpenOutputDir);
            this.Controls.Add(this.buttonOutputDirDetect);
            this.Controls.Add(this.button7ZipPathDetect);
            this.Controls.Add(this.labelOutputDir);
            this.Controls.Add(this.textBoxOutputDir);
            this.Controls.Add(this.buttonOutputDirBrowse);
            this.Controls.Add(this.label7ZipPath);
            this.Controls.Add(this.textBox7ZipPath);
            this.Controls.Add(this.button7ZipPathBrowse);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelPercent);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelStatusText);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.labelArchivePath);
            this.Controls.Add(this.textBoxArchivePath);
            this.Controls.Add(this.buttonArchiveBrowse);
            this.Controls.Add(this.progressBarCompression);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "SevenZipForm";
            this.ShowIcon = false;
            this.Text = "解压缩";
            this.groupBoxExtraOptions.ResumeLayout(false);
            this.groupBoxExtraOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarCompression;
        private System.Windows.Forms.Button buttonArchiveBrowse;
        private System.Windows.Forms.Label labelArchivePath;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelStatusText;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelPercent;
        public System.Windows.Forms.TextBox textBoxArchivePath;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label7ZipPath;
        public System.Windows.Forms.TextBox textBox7ZipPath;
        private System.Windows.Forms.Button button7ZipPathBrowse;
        private System.Windows.Forms.Label labelOutputDir;
        public System.Windows.Forms.TextBox textBoxOutputDir;
        private System.Windows.Forms.Button buttonOutputDirBrowse;
        private System.Windows.Forms.Button button7ZipPathDetect;
        private System.Windows.Forms.Button buttonOutputDirDetect;
        private System.Windows.Forms.Button buttonOpenOutputDir;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.GroupBox groupBoxExtraOptions;
        private System.Windows.Forms.RadioButton radioBtnOverwriteAll;
        private System.Windows.Forms.RadioButton radioBtnRenameExisting;
        private System.Windows.Forms.RadioButton radioBtnRenameNewer;
        private System.Windows.Forms.RadioButton radioBtnSkipExisting;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label labelIncludeFiles;
        private System.Windows.Forms.TextBox textBoxIncludeFiles;
        private System.Windows.Forms.Button buttonDownload7Zip;
    }
}