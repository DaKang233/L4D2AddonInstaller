using L4D2AddonInstaller.WinUi3.Infrastructure;
using L4D2AddonInstaller.WinUi3.Models;
using L4D2AddonInstaller.WinUi3.Services;
using L4D2AddonInstaller.WinUi3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace L4D2AddonInstaller.WinUi3;

public sealed partial class MainWindow : Window, IUserDialogService, IFileDialogService
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        Title = $"《求生之路 2》附加组件安装器 v{typeof(App).Assembly.GetName().Version}";

        ViewModel = new MainViewModel(
            new AddonInstallService(),
            new SevenZipService(),
            new SystemIntegrationService(),
            this,
            this);

        if (Content is FrameworkElement fe)
            fe.DataContext = ViewModel;
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(Views.HomePage));
                    break;

                case "install":
                    ContentFrame.Navigate(typeof(Views.InstallPage));
                    break;

                case "extract":
                    ContentFrame.Navigate(typeof(Views.ExtractPage));
                    break;
            }
        }
    }

    public async Task ShowInfoAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "确定",
            XamlRoot = Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    public Task ShowErrorAsync(string title, string message) => ShowInfoAsync(title, message);

    public async Task<bool> ConfirmAsync(string title, string message, string primaryButtonText = "确定")
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = primaryButtonText,
            CloseButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<string?> PickFolderAsync()
    {
        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");
        InitializePicker(picker);
        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }

    public async Task<string[]?> PickArchivesAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".7z");
        picker.FileTypeFilter.Add(".zip");
        picker.FileTypeFilter.Add(".rar");
        picker.FileTypeFilter.Add(".tar");
        picker.FileTypeFilter.Add(".gz");
        picker.FileTypeFilter.Add(".bz2");
        picker.FileTypeFilter.Add(".xz");
        picker.FileTypeFilter.Add(".iso");
        InitializePicker(picker);

        var files = await picker.PickMultipleFilesAsync();
        return files?.Select(f => f.Path).ToArray();
    }

    public async Task<string?> PickSevenZipExeAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".exe");
        InitializePicker(picker);
        var file = await picker.PickSingleFileAsync();
        return file?.Path;
    }

    private void InitializePicker(object picker)
    {
        var hwnd = Win32Interop.GetWindowHandle(this);
        switch (picker)
        {
            case FileOpenPicker filePicker:
                WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);
                break;
            case FolderPicker folderPicker:
                WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
                break;
        }
    }
}
