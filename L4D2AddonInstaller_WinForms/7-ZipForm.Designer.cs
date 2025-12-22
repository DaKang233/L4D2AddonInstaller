namespace L4D2AddonInstaller_WinForms
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
            this.buttonStop = new System.Windows.Forms.Button();
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
            this.labelArchivePath.Size = new System.Drawing.Size(79, 20);
            this.labelArchivePath.TabIndex = 3;
            this.labelArchivePath.Text = "压缩包路径";
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
            // buttonStop
            // 
            this.buttonStop.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonStop.Location = new System.Drawing.Point(199, 13);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(88, 30);
            this.buttonStop.TabIndex = 9;
            this.buttonStop.Text = "终止";
            this.buttonStop.UseVisualStyleBackColor = true;
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
            // SevenZipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 279);
            this.Controls.Add(this.buttonOpenOutputDir);
            this.Controls.Add(this.buttonOutputDirDetect);
            this.Controls.Add(this.button7ZipPathDetect);
            this.Controls.Add(this.labelOutputDir);
            this.Controls.Add(this.textBoxOutputDir);
            this.Controls.Add(this.buttonOutputDirBrowse);
            this.Controls.Add(this.label7ZipPath);
            this.Controls.Add(this.textBox7ZipPath);
            this.Controls.Add(this.button7ZipPathBrowse);
            this.Controls.Add(this.buttonStop);
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
        private System.Windows.Forms.Button buttonStop;
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
    }
}