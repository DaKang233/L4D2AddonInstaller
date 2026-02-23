using System;
using System.Threading;
using System.Threading.Tasks;
using static InstallerForL4D2AddonInstaller.Parser.SteamLibraryVdfParser;

namespace InstallerForL4D2AddonInstaller.Services
{
    public interface IInstallService
    {
        Task InstallAsync(string installRootPath, VersionDetails versionDetails, IProgress<InstallProgressInfo> progress, CancellationToken cancellationToken);
    }
}
