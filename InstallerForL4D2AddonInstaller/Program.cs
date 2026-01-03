using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallerForL4D2AddonInstaller
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Environment.OSVersion.Version < new Version(10, 0, 17763))
            {
                MessageBox.Show("需要 Windows 10 1809 或更高版本","错误",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            Application.Run(new InstallerForm());
        }
    }
}
