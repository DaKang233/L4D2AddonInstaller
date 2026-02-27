using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace L4D2AddonInstaller
{
    internal static class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [DllImport("ntdll.dll")]
        private static extern int RtlGetVersion(ref OSVERSIONINFOEX lpVersionInformation);

        private static Version GetWindowsVersion()
        {
            OSVERSIONINFOEX osvi = new OSVERSIONINFOEX();
            osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);

            if (RtlGetVersion(ref osvi) != 0)
            {
                throw new System.ComponentModel.Win32Exception();
            }

            return new Version((int)osvi.dwMajorVersion, (int)osvi.dwMinorVersion, (int)osvi.dwBuildNumber);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Version osVersion = GetWindowsVersion();
                Console.WriteLine(osVersion); // 例如: 10.0.19045

                if (osVersion < new Version(10, 0, 17763))
                {
                    MessageBox.Show("需要 Windows 10 1809 或更高版本", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取操作系统版本时出错: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Application.Run(new InstallerForm());
        }
    }
}