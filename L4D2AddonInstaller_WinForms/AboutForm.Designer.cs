namespace L4D2AddonInstaller_WinForms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelAppTitle = new System.Windows.Forms.Label();
            this.labelDeveloper = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelAppDesc = new System.Windows.Forms.Label();
            this.labelContact = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::L4D2AddonInstaller_WinForms.Properties.Resources.authorImage;
            this.pictureBox1.Location = new System.Drawing.Point(649, 38);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(114, 114);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelAppTitle
            // 
            this.labelAppTitle.AutoSize = true;
            this.labelAppTitle.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelAppTitle.Location = new System.Drawing.Point(40, 38);
            this.labelAppTitle.Name = "labelAppTitle";
            this.labelAppTitle.Size = new System.Drawing.Size(311, 26);
            this.labelAppTitle.TabIndex = 1;
            this.labelAppTitle.Text = "Left 4 Daed 2 Add-ons Installer";
            // 
            // labelDeveloper
            // 
            this.labelDeveloper.AutoSize = true;
            this.labelDeveloper.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelDeveloper.Location = new System.Drawing.Point(40, 64);
            this.labelDeveloper.Name = "labelDeveloper";
            this.labelDeveloper.Size = new System.Drawing.Size(229, 20);
            this.labelDeveloper.TabIndex = 2;
            this.labelDeveloper.Text = "Developer: DaKang233 (大康233)";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelVersion.Location = new System.Drawing.Point(40, 84);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(96, 20);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Version: 1.0.0";
            // 
            // labelAppDesc
            // 
            this.labelAppDesc.AutoSize = true;
            this.labelAppDesc.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelAppDesc.Location = new System.Drawing.Point(40, 104);
            this.labelAppDesc.Name = "labelAppDesc";
            this.labelAppDesc.Size = new System.Drawing.Size(487, 20);
            this.labelAppDesc.TabIndex = 4;
            this.labelAppDesc.Text = "A tool that helps user to install L4D2 Add-ons very in a very simple way.";
            // 
            // labelContact
            // 
            this.labelContact.AutoSize = true;
            this.labelContact.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelContact.Location = new System.Drawing.Point(40, 124);
            this.labelContact.Name = "labelContact";
            this.labelContact.Size = new System.Drawing.Size(264, 20);
            this.labelContact.TabIndex = 5;
            this.labelContact.Text = "Contact: dakang233@dakang233.com";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 187);
            this.Controls.Add(this.labelContact);
            this.Controls.Add(this.labelAppDesc);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelDeveloper);
            this.Controls.Add(this.labelAppTitle);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Text = "关于";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelAppTitle;
        private System.Windows.Forms.Label labelDeveloper;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelAppDesc;
        private System.Windows.Forms.Label labelContact;
    }
}