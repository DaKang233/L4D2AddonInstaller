namespace L4D2AddonInstaller
{
    partial class InstallPathStep
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxVersions = new System.Windows.Forms.ListBox();
            this.richTextBoxVersionDetails = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textBoxInstallPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonReload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxVersions
            // 
            this.listBoxVersions.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxVersions.FormattingEnabled = true;
            this.listBoxVersions.ItemHeight = 17;
            this.listBoxVersions.Location = new System.Drawing.Point(4, 85);
            this.listBoxVersions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBoxVersions.Name = "listBoxVersions";
            this.listBoxVersions.Size = new System.Drawing.Size(127, 140);
            this.listBoxVersions.TabIndex = 12;
            this.listBoxVersions.SelectedIndexChanged += new System.EventHandler(this.listBoxVersions_SelectedIndexChanged);
            // 
            // richTextBoxVersionDetails
            // 
            this.richTextBoxVersionDetails.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxVersionDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxVersionDetails.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.richTextBoxVersionDetails.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxVersionDetails.Location = new System.Drawing.Point(139, 85);
            this.richTextBoxVersionDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richTextBoxVersionDetails.Name = "richTextBoxVersionDetails";
            this.richTextBoxVersionDetails.ReadOnly = true;
            this.richTextBoxVersionDetails.Size = new System.Drawing.Size(346, 140);
            this.richTextBoxVersionDetails.TabIndex = 11;
            this.richTextBoxVersionDetails.TabStop = false;
            this.richTextBoxVersionDetails.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(-5, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(335, 25);
            this.label2.TabIndex = 10;
            this.label2.Text = "选择要安装的版本，并指定安装目录。";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowse.Location = new System.Drawing.Point(385, 239);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 33);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.Text = "浏览";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // textBoxInstallPath
            // 
            this.textBoxInstallPath.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxInstallPath.Location = new System.Drawing.Point(4, 242);
            this.textBoxInstallPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxInstallPath.Name = "textBoxInstallPath";
            this.textBoxInstallPath.Size = new System.Drawing.Size(373, 26);
            this.textBoxInstallPath.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(-5, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 28);
            this.label1.TabIndex = 7;
            this.label1.Text = "选择安装位置";
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(378, 36);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(107, 33);
            this.buttonReload.TabIndex = 13;
            this.buttonReload.Text = "重载版本信息";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // InstallPathStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.listBoxVersions);
            this.Controls.Add(this.richTextBoxVersionDetails);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.textBoxInstallPath);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "InstallPathStep";
            this.Size = new System.Drawing.Size(489, 282);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxVersions;
        private System.Windows.Forms.RichTextBox richTextBoxVersionDetails;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox textBoxInstallPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonReload;
    }
}
