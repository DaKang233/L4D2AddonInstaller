namespace L4D2AddonInstaller.WinUi3.Services;
using System.Threading.Tasks;

public interface IFileDialogService
{
    Task<string?> PickFolderAsync();
    Task<string[]?> PickArchivesAsync();
    Task<string?> PickSevenZipExeAsync();
}
