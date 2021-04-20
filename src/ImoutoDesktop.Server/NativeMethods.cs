using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ImoutoDesktop.Server
{
    static class NativeMethods
    {
        public const int Force = 0x00000004;

        public const int ForceIfHung = 0x00000010;

        public const int Logoff = 0x00000000;

        public const int Poweroff = 0x00000008;

        public const int Reboot = 0x00000002;

        public const int Shutdown = 0x00000001;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ExitWindowsEx(int flag, int reserved);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct LUID_AND_ATTRIBUTES
        {
            public long Luid;
            public int Attributes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            int DesiredAccess,
            ref IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(
            string lpSystemName,
            string lpName,
            ref long lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            int BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength
            );

        public static bool ExitWindows(int flag)
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetShutdownPrivilege();
            }
            bool result = ExitWindowsEx(flag, 0);
            return result;
        }

        private static void SetShutdownPrivilege()
        {
            const int TOKEN_QUERY = 0x00000008;
            const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
            const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
            const int SE_PRIVILEGE_ENABLED = 0x00000002;

            // プロセスのハンドルを取得する。
            IntPtr hproc = System.Diagnostics.Process.GetCurrentProcess().Handle;
            // IntPtr hproc = GetCurrentProcess(); // この方法でもＯＫ．

            // Token を取得する。
            IntPtr hToken = IntPtr.Zero;
            OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken);

            // LUID を取得する。
            long luid = 0;
            LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref luid);

            // 特権を設定する。
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
            tp.PrivilegeCount = 1;
            tp.Privileges = new LUID_AND_ATTRIBUTES();
            tp.Privileges.Luid = luid;
            tp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

            // 特権をセットする。
            AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        }

    }
}
