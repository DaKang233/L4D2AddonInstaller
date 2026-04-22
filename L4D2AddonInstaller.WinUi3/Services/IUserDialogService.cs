using System.Threading.Tasks;
using System;
using System.Collections.Generic;
namespace L4D2AddonInstaller.WinUi3.Services;

public interface IUserDialogService
{
    Task ShowInfoAsync(string title, string message);
    Task ShowErrorAsync(string title, string message);
    Task<bool> ConfirmAsync(string title, string message, string primaryButtonText = "确定");
}
