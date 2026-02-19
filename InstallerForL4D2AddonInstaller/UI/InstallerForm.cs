using InstallerForL4D2AddonInstaller.Parser;
using InstallerForL4D2AddonInstaller.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static InstallerForL4D2AddonInstaller.InstallPathStep;

namespace InstallerForL4D2AddonInstaller
{
    public partial class InstallerForm : Form
    {
        private List<UserControl> _steps;
        private int _currentStepIndex = 0;

        private WelcomeStep welcomeStep;
        private InstallLicense installLicense;
        private InstallPathStep installPathStep;
        private InstallProgressStep installProgressStep;
        private FinishedStep finishedStep;

        public SteamLibraryVdfParser.VersionDetails SelectedVersionDetail;
        public string InstallPath;
        public EventHandler<bool> InstallationStarted;
        public EventHandler InstallationFinished;
        public bool IsClosedWithoutAsking = false;
        public CancellationTokenSource cts;
        public const string InstallerAppName = "L4D2AddonInstaller";

        public InstallerForm()
        {
            InitializeComponent();

            welcomeStep = new WelcomeStep();
            installLicense = new InstallLicense(this);
            installPathStep = new InstallPathStep(this);
            installProgressStep = new InstallProgressStep(this);
            finishedStep = new FinishedStep(this);

            // 初始化步骤列表
            _steps = new List<UserControl>
        {
            welcomeStep,
            installLicense,
            installPathStep,
            installProgressStep,
            finishedStep
        };

            // 显示第一步
            ShowStep(_currentStepIndex);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            installProgressStep.InstallationCompleted += (s, args) =>
            {
                StepIndexIncrement();
            };
            installPathStep.BtnNextEvent += (s, args) =>
            {
                if (args.EnableNext)
                {
                    btnNext.Enabled = true;
                    VersionsLoaded = true;
                }
                else
                    btnNext.Enabled = false;
            };

            installLicense.AgreementCheckedChanged += (s, agreed) =>
            {
                btnNext.Enabled = agreed;
                AgreementChecked = agreed;
            };
        }

        bool VersionsLoaded = false;
        bool AgreementChecked = false;
        private void ShowStep(int index)
        {
            // 清除当前容器内容
            pnlStepContainer.Controls.Clear();

            if (index >= 0 && index < _steps.Count)
            {
                var step = _steps[index];
                step.Dock = DockStyle.Fill;
                pnlStepContainer.Controls.Add(step);
                bool isProgressStep = step is InstallProgressStep;
                bool isPathStep = step is InstallPathStep;

                // 更新按钮状态
                btnPrev.Enabled = index > 0 && !isProgressStep;
                btnNext.Enabled = !isProgressStep && (isPathStep ? VersionsLoaded : true);

                // 如果是最后一步，按钮文字改为“安装”
                if (isPathStep)
                {
                    btnNext.Text = "安装";
                }
                else
                {
                    btnNext.Text = "下一步";
                }
                if (step.Name == "InstallLicense")
                    btnNext.Enabled = AgreementChecked;

                if (step.Name == "FinishedStep")
                {
                    btnPrev.Enabled = false;
                    btnCancel.Enabled = false;
                    btnNext.Text = "完成";
                }
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentStepIndex > 0)
            {
                _currentStepIndex--;
                ShowStep(_currentStepIndex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_steps[_currentStepIndex] is InstallPathStep)
            {

                // 最后一步，开始安装
                if (MessageBox.Show("确定要安装吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _currentStepIndex++;
                    InstallationStarted?.Invoke(this, true);
                    ShowStep(_currentStepIndex);
                }
            }
            else if (_currentStepIndex < _steps.Count)
            {
                _currentStepIndex++;
                ShowStep(_currentStepIndex);
            }
            if (_currentStepIndex == _steps.Count)
            {
                InstallationFinished?.Invoke(this, e);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!IsClosedWithoutAsking)
                if (MessageBox.Show("确定要取消安装吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    cts?.Cancel();
                    cts?.Dispose();
                    base.OnFormClosing(e);
                }
            else base.OnFormClosing(e);
        }

        private void StepIndexIncrement()
        {
            _currentStepIndex++;
            ShowStep(_currentStepIndex);
        }
    }
}
