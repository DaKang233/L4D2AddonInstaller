using System;
using System.Threading;
using System.Threading.Tasks;
using static L4D2AddonInstaller.Parsers.SteamLibraryVdfParser;

namespace L4D2AddonInstaller.Services
{
    public interface IInstallService
    {
        Task InstallAsync(string installRootPath, VersionDetails versionDetails, IProgress<InstallProgressInfo> progress, CancellationToken cancellationToken);
    }
}
