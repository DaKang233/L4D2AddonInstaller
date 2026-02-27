namespace L4D2AddonInstaller.UI
{
    partial class InstallLicense
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLblLicense = new System.Windows.Forms.LinkLabel();
            this.checkBoxAgreement = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(-5, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(392, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "请阅读并同意以下许可协议以继续安装程序。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(-5, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 28);
            this.label1.TabIndex = 5;
            this.label1.Text = "许可协议";
            // 
            // linkLblLicense
            // 
            this.linkLblLicense.AutoSize = true;
            this.linkLblLicense.Location = new System.Drawing.Point(3, 109);
            this.linkLblLicense.Name = "linkLblLicense";
            this.linkLblLicense.Size = new System.Drawing.Size(163, 20);
            this.linkLblLicense.TabIndex = 7;
            this.linkLblLicense.TabStop = true;
            this.linkLblLicense.Text = "点击此处阅读许可协议。";
            this.linkLblLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblLicense_LinkClicked);
            // 
            // checkBoxAgreement
            // 
            this.checkBoxAgreement.AutoSize = true;
            this.checkBoxAgreement.Location = new System.Drawing.Point(7, 169);
            this.checkBoxAgreement.Name = "checkBoxAgreement";
            this.checkBoxAgreement.Size = new System.Drawing.Size(224, 24);
            this.checkBoxAgreement.TabIndex = 8;
            this.checkBoxAgreement.Text = "我已阅读并同意遵守许可协议。";
            this.checkBoxAgreement.UseVisualStyleBackColor = true;
            this.checkBoxAgreement.CheckedChanged += new System.EventHandler(this.checkBoxAgreement_CheckedChanged);
            // 
            // InstallLicense
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.checkBoxAgreement);
            this.Controls.Add(this.linkLblLicense);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "InstallLicense";
            this.Size = new System.Drawing.Size(489, 282);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLblLicense;
        private System.Windows.Forms.CheckBox checkBoxAgreement;
    }
}
