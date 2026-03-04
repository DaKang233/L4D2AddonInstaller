using System;
using System.Threading;
using System.Threading.Tasks;


namespace L4D2AddonInstaller.Services
{
    public interface IAddonInstallService
    {
        Task<AddonInstallProgressInfo> ResolveServerInfoAsync(string code, CancellationToken cancellationToken);
        Task<AddonInstallProgressInfo> DownloadAndInstallAsync(string code, string gamePath, IProgress<AddonInstallProgressInfo> progress, CancellationToken cancellationToken);
    }
}
