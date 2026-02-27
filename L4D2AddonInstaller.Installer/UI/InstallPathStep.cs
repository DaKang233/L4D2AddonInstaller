using L4D2AddonInstaller.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using static L4D2AddonInstaller.Parsers.SteamLibraryVdfParser;

namespace L4D2AddonInstaller
{
    public partial class InstallPathStep : UserControl
    {
        private InstallerForm installerForm;
        public InstallPathStep(InstallerForm form)
        {
            InitializeComponent();
            installerForm = form;
        }

        public class BtnNextEventArgrs : EventArgs
        {
            public bool EnableNext { get; set; }
            public bool VersionsLoaded { get; set; }
        }

        public event EventHandler<BtnNextEventArgrs> BtnNextEvent;
        private BtnNextEventArgrs _btnNextEventFalse = new BtnNextEventArgrs() { EnableNext = false, VersionsLoaded = false };
        private BtnNextEventArgrs _btnNextEventTrue = new BtnNextEventArgrs() { EnableNext = true, VersionsLoaded = true };

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadVersionsAsync();
            installerForm.InstallationStarted += InstallerForm_InstallationStarted;
        }

        private void InstallerForm_InstallationStarted(object sender, bool e)
        {
            installerForm.InstallPath = textBoxInstallPath.Text;
        }

        private Dictionary<string, VersionDetails> _version;

        const string versionListUrl = "https://furina.dakang233.com:8443/www/l4d2/installer/installerVersion.vdf";
        private async Task<Dictionary<string, VersionDetails>> GetVersionList()
        {
            string versionListText;
            try { versionListText = await HttpHelper.GetRemoteTextAsync(versionListUrl); } catch { throw; }
            if (string.IsNullOrEmpty(versionListText))
            {
                throw new Exception("Failed to get version list from server.");
            }
            var versionList = GetVersionDetails(versionListText);
            return versionList;
        }

        private async Task DisplayVersions()
        {
            _version = await GetVersionList();
            listBoxVersions.DisplayMember = "Version";
            listBoxVersions.DataSource = _version.Values.ToList();
        }

        private void listBoxVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxVersions.SelectedItem is VersionDetails details)
            {
                ShowVersionDetails(details);
                installerForm.SelectedVersionDetail = details;
            }
        }

        private void ShowVersionDetails(VersionDetails v)
        {
            richTextBoxVersionDetails.Clear();

            richTextBoxVersionDetails.AppendText(
        $@"版本号：{v.Version}
名称：{v.Name}
类型：{v.Type}
作者：{v.Author}

描述：
{v.Description}
");
            /*下载信息：
{v.Protocol}://{v.Prefix}.{v.WebServer}:{v.WebPort}/{v.RelativePath}

7-Zip：
EXE: {v.SevenZipExePath}
DLL: {v.SevenZipDllPath}*/
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择安装目录";
                fbd.ShowNewFolderButton = true;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBoxInstallPath.Text = fbd.SelectedPath;
                }
            }
        }

        private async Task LoadVersionsAsync()
        {
            try
            {
                listBoxVersions.DataSource = null;
                richTextBoxVersionDetails.Clear();
                listBoxVersions.SelectedIndexChanged -= listBoxVersions_SelectedIndexChanged;
                buttonReload.Enabled = false;
                BtnNextEvent?.Invoke(installerForm, _btnNextEventFalse);
                textBoxInstallPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                await DisplayVersions();
                BtnNextEvent?.Invoke(installerForm, _btnNextEventTrue);
                listBoxVersions.SelectedIndexChanged += listBoxVersions_SelectedIndexChanged;
                if (listBoxVersions.Items.Count > 0)
                {
                    listBoxVersions.SelectedIndex = 0;
                    listBoxVersions_SelectedIndexChanged(this, EventArgs.Empty);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("获取版本列表失败，请检查计算机是否已连接到互联网：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                richTextBoxVersionDetails.AppendText("获取失败。\n");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取版本列表失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                richTextBoxVersionDetails.AppendText("获取失败。\n");
                return;
            }
            finally
            {
                buttonReload.Enabled = true;
            }
        }

        private async void buttonReload_Click(object sender, EventArgs e)
        {
            await LoadVersionsAsync();
        }
    }
}
