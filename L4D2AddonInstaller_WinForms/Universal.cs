using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace L4D2AddonInstaller_WinForms
{
    /// <summary>
    /// 通用工具类
    /// </summary>
    public class Universal
    {
        /// <summary>
        /// 获取应用程序的当前版本号
        /// </summary>
        /// <returns>应用程序的版本号字符串</returns>
        public static string GetAppVersion()
        {
            return System.Windows.Forms.Application.ProductVersion;
        }

        /// <summary>
        /// 尝试获取 GB18030 编码，如果不可用则返回 UTF-8 编码
        /// </summary>
        /// <returns>Encoding 类（G18030或UTF8）</returns>
        public static Encoding TryGetGB18030Encoding()
        {
            try
            {
                return Encoding.GetEncoding("GB18030");
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// 递归查找控件的通用方法
        /// </summary>
        /// <param name="parent">控件组</param>
        /// <param name="controlName">控件名</param>
        /// <returns></returns>
        public static Control FindControlRecursive(Control parent, string controlName)
        {
            if (parent == null || string.IsNullOrEmpty(controlName))
                return null;

            // 先检查当前控件的直接子控件
            Control target = parent.Controls[controlName];
            if (target != null)
                return target;

            // 递归遍历所有子容器中的控件
            foreach (Control child in parent.Controls)
            {
                target = FindControlRecursive(child, controlName);
                if (target != null)
                    return target;
            }

            return null; // 未找到
        }
    }
}
