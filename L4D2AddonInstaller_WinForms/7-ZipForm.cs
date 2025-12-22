using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace L4D2AddonInstaller_WinForms
{
    public partial class SevenZipForm : Form
    {
        private MainForm mainForm;
        public SevenZipForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
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
    }
}
