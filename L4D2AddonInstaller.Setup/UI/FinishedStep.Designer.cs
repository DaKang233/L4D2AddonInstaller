namespace L4D2AddonInstaller.UI
{
    partial class FinishedStep
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxStart = new System.Windows.Forms.CheckBox();
            this.checkBoxCreateShortcut = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(-5, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 28);
            this.label1.TabIndex = 2;
            this.label1.Text = "安装已完成！";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(-5, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(316, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "点击「完成」以完成本次安装向导。";
            // 
            // checkBoxStart
            // 
            this.checkBoxStart.AutoSize = true;
            this.checkBoxStart.Checked = true;
            this.checkBoxStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStart.Location = new System.Drawing.Point(3, 129);
            this.checkBoxStart.Name = "checkBoxStart";
            this.checkBoxStart.Size = new System.Drawing.Size(182, 24);
            this.checkBoxStart.TabIndex = 9;
            this.checkBoxStart.Text = "点击「完成」时启动程序";
            this.checkBoxStart.UseVisualStyleBackColor = true;
            // 
            // checkBoxCreateShortcut
            // 
            this.checkBoxCreateShortcut.AutoSize = true;
            this.checkBoxCreateShortcut.Checked = true;
            this.checkBoxCreateShortcut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreateShortcut.Location = new System.Drawing.Point(3, 159);
            this.checkBoxCreateShortcut.Name = "checkBoxCreateShortcut";
            this.checkBoxCreateShortcut.Size = new System.Drawing.Size(210, 24);
            this.checkBoxCreateShortcut.TabIndex = 10;
            this.checkBoxCreateShortcut.Text = "在「开始」菜单创建快捷方式";
            this.checkBoxCreateShortcut.UseVisualStyleBackColor = true;
            // 
            // FinishedStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.checkBoxCreateShortcut);
            this.Controls.Add(this.checkBoxStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FinishedStep";
            this.Size = new System.Drawing.Size(489, 282);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxStart;
        private System.Windows.Forms.CheckBox checkBoxCreateShortcut;
    }
}
