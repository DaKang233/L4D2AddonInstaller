using System.Windows.Forms;

namespace L4D2AddonInstaller
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
