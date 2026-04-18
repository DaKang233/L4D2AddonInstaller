using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using L4D2AddonInstaller.WinUi3.Models;
using L4D2AddonInstaller.WinUi3.Services;

namespace L4D2AddonInstaller.WinUi3.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AddonInstallService _installService;
    private readonly SevenZipService _sevenZipService;
    private readonly SystemIntegrationService _systemService;
    private readonly IUserDialogService _dialogService;
    private readonly IFileDialogService _fileDialogService;

    private CancellationTokenSource? _downloadCts;
    private CancellationTokenSource? _extractCts;

    [ObservableProperty] private string steamPath = string.Empty;
    [ObservableProperty] private string gamePath = string.Empty;
    [ObservableProperty] private string codeName = string.Empty;
    [ObservableProperty] private string serverInfo = string.Empty;
    [ObservableProperty] private string consoleCommand = string.Empty;
    [ObservableProperty] private string statusMessage = "就绪";
    [ObservableProperty] private bool autoStartGame;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool canCancelDownload;
    [ObservableProperty] private int downloadPercent;

    [ObservableProperty] private string archivePaths = string.Empty;
    [ObservableProperty] private string sevenZipPath = string.Empty;
    [ObservableProperty] private string outputDir = string.Empty;
    [ObservableProperty] private int extractPercent;
    [ObservableProperty] private OverwriteMode overwriteMode = OverwriteMode.OverwriteAll;

    public IReadOnlyList<OverwriteMode> OverwriteModes { get; } = Enum.GetValues<OverwriteMode>();

    public MainViewModel(AddonInstallService installService, SevenZipService sevenZipService, SystemIntegrationService systemService, IUserDialogService dialogService, IFileDialogService fileDialogService)
    {
        _installService = installService;
        _sevenZipService = sevenZipService;
        _systemService = systemService;
        _dialogService = dialogService;
        _fileDialogService = fileDialogService;

        SevenZipPath = _sevenZipService.Detect7ZipPath() ?? string.Empty;
    }

    [RelayCommand]
    private async Task BrowseSteamPathAsync()
    {
        var path = await _fileDialogService.PickFolderAsync();
        if (path is not null) SteamPath = path;
    }

    [RelayCommand]
    private async Task BrowseGamePathAsync()
    {
        var path = await _fileDialogService.PickFolderAsync();
        if (path is not null) GamePath = path;
    }

    [RelayCommand]
    private void DetectSteamPath()
    {
        var path = _systemService.GetSteamPathFromRegistry();
        if (!string.IsNullOrWhiteSpace(path))
            SteamPath = Path.GetFullPath(path);
    }

    [RelayCommand]
    private async Task DetectGamePathAsync()
    {
        if (!Directory.Exists(SteamPath))
        {
            await _dialogService.ShowInfoAsync("提示", "请先设置正确的 Steam 路径。");
            return;
        }

        var vdf = Path.Combine(SteamPath, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(vdf))
        {
            await _dialogService.ShowInfoAsync("提示", "未找到 libraryfolders.vdf。");
            return;
        }

        var libraryPath = SteamLibraryVdfParserModern.GetLibraryPathByGameId(vdf, "550");
        if (string.IsNullOrWhiteSpace(libraryPath))
        {
            await _dialogService.ShowInfoAsync("提示", "未在 Steam 库中找到 Left 4 Dead 2。");
            return;
        }

        GamePath = Path.Combine(libraryPath, "steamapps", "common", "Left 4 Dead 2");
        OutputDir = Path.Combine(GamePath, "left4dead2", "addons");
    }

    [RelayCommand]
    private async Task DetectAllPathAsync()
    {
        DetectSteamPath();
        await DetectGamePathAsync();
    }

    [RelayCommand]
    private async Task ResolveServerInfoAsync()
    {
        if (string.IsNullOrWhiteSpace(CodeName))
        {
            await _dialogService.ShowInfoAsync("提示", "请输入下载代号（如1、231）。");
            return;
        }

        try
        {
            var info = await _installService.ResolveServerInfoAsync(CodeName, CancellationToken.None);
            ServerInfo = info.ServerDisplay;
            ConsoleCommand = info.ConnectCommand;
            StatusMessage = "服务器信息获取成功";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("错误", $"获取服务器信息失败：{ex.Message}");
        }
    }

    [RelayCommand]
    private async Task StartSteamAsync()
    {
        try
        {
            var exe = Path.Combine(SteamPath, "steam.exe");
            if (!File.Exists(exe))
            {
                await _dialogService.ShowErrorAsync("错误", "Steam 路径无效。请先设置正确路径。");
                return;
            }

            _systemService.StartExecutable(exe);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("错误", $"启动 Steam 失败：{ex.Message}");
        }
    }

    [RelayCommand]
    private async Task StartGameAsync()
    {
        var host = ServerInfo.Split(':').FirstOrDefault()?.Trim() ?? string.Empty;
        var port = ServerInfo.Contains(':') ? ServerInfo.Split(':').Last().Trim() : string.Empty;
        if (string.IsNullOrWhiteSpace(host))
        {
            await _dialogService.ShowInfoAsync("提示", "请先解析代号获取服务器信息。");
            return;
        }

        await StartGameCoreAsync(host, port);
    }

    [RelayCommand]
    private async Task InstallByCodeAsync()
    {
        if (!await ValidateInstallInputsAsync())
            return;

        IsBusy = true;
        CanCancelDownload = true;
        DownloadPercent = 0;
        StatusMessage = "正在获取下载配置...";
        _downloadCts = new CancellationTokenSource();

        try
        {
            var progress = new Progress<AddonInstallProgressInfo>(p =>
            {
                StatusMessage = p.StatusMessage;
                DownloadPercent = Math.Clamp(p.Percent, 0, 100);
                if (!string.IsNullOrWhiteSpace(p.ServerDisplay))
                {
                    ServerInfo = p.ServerDisplay;
                    ConsoleCommand = p.ConnectCommand;
                }
            });

            var result = await _installService.DownloadAndInstallAsync(CodeName, GamePath, progress, _downloadCts.Token);

            var extracted = false;
            if (result.ContainsArchive)
            {
                var needExtract = await _dialogService.ConfirmAsync("提示", "检测到压缩包，是否立即解压到 addons 目录？");
                if (needExtract)
                {
                    ArchivePaths = string.Join(';', result.DownloadedArchivePaths);
                    OutputDir = Path.Combine(GamePath, "left4dead2", "addons");
                    extracted = await ExtractArchivesInternalAsync(result.DownloadedArchivePaths);
                }
            }

            StatusMessage = extracted ? "所有附加组件下载并安装完成！" : result.StatusMessage;
            if (AutoStartGame && !string.IsNullOrWhiteSpace(result.Host) && (!result.ContainsArchive || extracted))
            {
                await StartGameCoreAsync(result.Host, result.Port);
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "下载已取消。";
        }
        catch (Exception ex)
        {
            StatusMessage = "下载/解析失败！";
            await _dialogService.ShowErrorAsync("错误", ex.Message);
        }
        finally
        {
            IsBusy = false;
            CanCancelDownload = false;
            _downloadCts?.Dispose();
            _downloadCts = null;
        }
    }

    [RelayCommand]
    private void CancelDownload()
    {
        _downloadCts?.Cancel();
    }

    [RelayCommand]
    private async Task OneClickFinishAllAsync()
    {
        await DetectAllPathAsync();
        await InstallByCodeAsync();
    }

    [RelayCommand]
    private void OpenArchiveDownloadFolder()
    {
        if (!Directory.Exists(GamePath))
            return;

        var path = Path.Combine(GamePath, "l4d2InstallToolDownloads");
        Directory.CreateDirectory(path);
        _systemService.OpenFolder(path);
    }

    [RelayCommand]
    private async Task BrowseArchivesAsync()
    {
        var files = await _fileDialogService.PickArchivesAsync();
        if (files is { Length: > 0 })
            ArchivePaths = string.Join(';', files);
    }

    [RelayCommand]
    private async Task BrowseSevenZipAsync()
    {
        var file = await _fileDialogService.PickSevenZipExeAsync();
        if (file is not null)
            SevenZipPath = file;
    }

    [RelayCommand]
    private async Task BrowseOutputDirAsync()
    {
        var path = await _fileDialogService.PickFolderAsync();
        if (path is not null)
            OutputDir = path;
    }

    [RelayCommand]
    private void DetectSevenZip()
    {
        SevenZipPath = _sevenZipService.Detect7ZipPath() ?? string.Empty;
    }

    [RelayCommand]
    private async Task DownloadSevenZipAsync()
    {
        _extractCts = new CancellationTokenSource();
        try
        {
            SevenZipPath = await _sevenZipService.Download7ZipAsync(_extractCts.Token, new Progress<int>(p => ExtractPercent = p));
            StatusMessage = "7-Zip 下载完成。";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("错误", $"下载 7-Zip 失败：{ex.Message}");
        }
        finally
        {
            _extractCts?.Dispose();
            _extractCts = null;
        }
    }

    [RelayCommand]
    private async Task ExtractArchivesAsync()
    {
        var archives = ArchivePaths.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        await ExtractArchivesInternalAsync(archives);
    }

    [RelayCommand]
    private void CancelExtract()
    {
        _extractCts?.Cancel();
    }

    private async Task<bool> ExtractArchivesInternalAsync(IEnumerable<string> archives)
    {
        var files = archives.Where(File.Exists).ToArray();
        if (files.Length == 0)
        {
            await _dialogService.ShowInfoAsync("提示", "请选择有效的压缩包。 ");
            return false;
        }

        if (!File.Exists(SevenZipPath))
        {
            await _dialogService.ShowInfoAsync("提示", "请先选择有效的 7z.exe 路径。 ");
            return false;
        }

        if (string.IsNullOrWhiteSpace(OutputDir))
        {
            await _dialogService.ShowInfoAsync("提示", "请先指定输出目录。 ");
            return false;
        }

        _extractCts = new CancellationTokenSource();
        ExtractPercent = 0;

        try
        {
            StatusMessage = "正在解压缩...";
            await _sevenZipService.ExtractAsync(files, OutputDir, SevenZipPath, OverwriteMode, _extractCts.Token, new Progress<int>(p => ExtractPercent = p));
            StatusMessage = "解压缩完成。";
            return true;
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "解压已取消。";
            return false;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("错误", ex.Message);
            return false;
        }
        finally
        {
            _extractCts?.Dispose();
            _extractCts = null;
        }
    }

    private async Task<bool> ValidateInstallInputsAsync()
    {
        if (string.IsNullOrWhiteSpace(CodeName))
        {
            await _dialogService.ShowInfoAsync("提示", "请输入下载代号（如1、231）。");
            return false;
        }

        if (!Directory.Exists(SteamPath) || !File.Exists(Path.Combine(SteamPath, "steam.exe")))
        {
            await _dialogService.ShowErrorAsync("错误", "Steam 路径无效，请确认路径正确。");
            return false;
        }

        if (!Directory.Exists(GamePath) || !File.Exists(Path.Combine(GamePath, "left4dead2.exe")))
        {
            await _dialogService.ShowErrorAsync("错误", "L4D2 路径无效，请确认路径正确。");
            return false;
        }

        return true;
    }

    private async Task StartGameCoreAsync(string host, string port)
    {
        try
        {
            if (!_systemService.IsProcessRunning("steam"))
            {
                var startSteam = await _dialogService.ConfirmAsync("提示", "检测到 Steam 未启动，是否尝试启动 Steam？", "启动");
                if (!startSteam)
                    return;

                var steamExe = Path.Combine(SteamPath, "steam.exe");
                _systemService.StartExecutable(steamExe);
                await _dialogService.ShowInfoAsync("提示", "请确认 Steam 已登录并完成启动后再继续。 ");
            }

            var connectArgs = string.IsNullOrWhiteSpace(port) ? host : $"{host}:{port}";
            _systemService.StartUri($"steam://rungameid/550//+connect {connectArgs}");
            StatusMessage = "游戏已启动！";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("错误", $"启动游戏失败：{ex.Message}");
            StatusMessage = "游戏启动失败！";
        }
    }
}
