using IWshRuntimeLibrary;
using System;
using System.IO;

namespace L4D2AddonInstaller.Helpers
{
    /// <summary>
    /// 快捷方式帮助类
    /// </summary>
    public class ShortcutHelper
    {
        /// <summary>
        /// 在开始菜单创建快捷方式
        /// </summary>
        /// <param name="targetExePath">目标程序路径（绝对路径）</param>
        /// <param name="shortcutName">快捷方式名称（不含扩展名）</param>
        /// <param name="description">快捷方式描述</param>
        /// <param name="workingDirectory">工作目录（null 则使用程序所在目录）</param>
        /// <param name="isAllUsers">是否创建到所有用户开始菜单（需要管理员权限）</param>
        public static void CreateStartMenuShortcut(
            string targetExePath,
            string shortcutName,
            string description = null,
            string workingDirectory = null,
            bool isAllUsers = false)
        {
            // 验证目标文件是否存在
            if (!System.IO.File.Exists(targetExePath))
                throw new FileNotFoundException("目标程序不存在", targetExePath);

            // 确定开始菜单路径
            string startMenuPath = isAllUsers
                ? Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
                : Environment.GetFolderPath(Environment.SpecialFolder.Programs);

            // 创建快捷方式文件路径
            string shortcutPath = Path.Combine(startMenuPath, $"{shortcutName}.lnk");

            // 创建 WshShell 对象
            WshShell shell = new WshShell();

            // 创建快捷方式
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetExePath;
            shortcut.WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(targetExePath);
            shortcut.Description = description ?? shortcutName;

            // 可选：设置图标（例如使用程序自身图标）
            // shortcut.IconLocation = targetExePath;

            // 保存快捷方式
            shortcut.Save();
        }
    }
}