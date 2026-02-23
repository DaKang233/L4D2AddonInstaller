using System;
using System.Threading;
using System.Threading.Tasks;


namespace L4D2AddonInstaller.Services
{
    public interface IInstallService
    {
        Task<InstallProgressInfo> ResolveServerInfoAsync(string code, CancellationToken cancellationToken);
        Task<InstallProgressInfo> DownloadAndInstallAsync(string code, string gamePath, IProgress<InstallProgressInfo> progress, CancellationToken cancellationToken);
    }
}
