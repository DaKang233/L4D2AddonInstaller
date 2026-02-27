using System;
using System.Windows.Forms;

namespace L4D2AddonInstaller.UI
{
    public partial class FinishedStep : UserControl
    {
        private InstallerForm installerForm;
        public FinishedStep(InstallerForm form)
        {
            InitializeComponent();
            installerForm = form;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            installerForm.InstallationFinished += FinishInstallation;
        }

        private void FinishInstallation(object sender, EventArgs e)
        {
            var installerFolder = System.IO.Path.Combine(installerForm.InstallPath, InstallerForm.InstallerAppName);
            if (checkBoxCreateShortcut.Checked)
            {
                try
                {
                    Helpers.ShortcutHelper.CreateStartMenuShortcut(
                        targetExePath: System.IO.Path.Combine(installerFolder, installerForm.SelectedVersionDetail.ExeName),
                        shortcutName: "《求生之路 2》附加组件安装器",
                        description: "一个用于帮助用户傻瓜式安装附加组件的程序。",
                        workingDirectory: System.IO.Path.Combine(installerFolder),
                        isAllUsers: false
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show("创建开始菜单快捷方式时出错：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (checkBoxStart.Checked)
            {
                Helpers.ProcessHelper.StartExecutable(
                    System.IO.Path.Combine(installerFolder, installerForm.SelectedVersionDetail.ExeName)
                );
            }
            CloseForm();
        }

        private void CloseForm()
        {
            installerForm.IsClosedWithoutAsking = true;
            installerForm.Close();
        }
    }
}
