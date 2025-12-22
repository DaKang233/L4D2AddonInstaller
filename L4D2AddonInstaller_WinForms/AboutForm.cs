using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L4D2AddonInstaller_WinForms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            labelVersion.Text = "Version: " + Application.ProductVersion;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            base.OnFormClosed(e);
        }
    }
}
