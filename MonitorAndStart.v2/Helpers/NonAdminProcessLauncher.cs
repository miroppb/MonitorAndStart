using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MonitorAndStart.v2.Helpers
{
    public static class NonAdminProcessLauncher
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool CreateProcessAsUser(
            IntPtr hToken, string? lpApplicationName, string lpCommandLine,
            IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles,
            uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes,
            int impersonationLevel, int tokenType, out IntPtr phNewToken);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            ref SECURITY_ATTRIBUTES lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            uint BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool LookupPrivilegeValue(string? lpSystemName, string lpName, out LUID lpLuid);

        [StructLayout(LayoutKind.Sequential)]
        struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        const uint SE_PRIVILEGE_ENABLED = 0x00000002;

        static bool EnablePrivilege(string privilege)
        {
            if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out IntPtr tokenHandle))
                return false;

            if (!LookupPrivilegeValue(null, privilege, out LUID luid))
                return false;

            TOKEN_PRIVILEGES tp = new()
            {
                PrivilegeCount = 1,
                Luid = luid,
                Attributes = SE_PRIVILEGE_ENABLED
            };

            bool result = AdjustTokenPrivileges(tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            CloseHandle(tokenHandle);
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_DUPLICATE = 0x0002;
        private const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        private const uint TOKEN_ALL_ACCESS = 0xF01FF;
        private const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        private const uint CREATE_NEW_CONSOLE = 0x00000010;
        private const int SecurityImpersonation = 2;
        private const int TokenPrimary = 1;
        private const uint STARTF_USESTDHANDLES = 0x00000100;

        // File access constants
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint CREATE_ALWAYS = 2;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        public static bool LaunchNonAdminProcess(string exePath, string args, string workingDir, string logFilePath)
        {
            Process? shellProcess = null;
            IntPtr logHandle = IntPtr.Zero;
            PROCESS_INFORMATION pi = new();

            try
            {
                shellProcess = Process.GetProcessesByName("explorer")[0];
                if (!OpenProcessToken(shellProcess.Handle, TOKEN_DUPLICATE | TOKEN_ASSIGN_PRIMARY | TOKEN_QUERY, out IntPtr shellToken))
                    throw new Exception("Failed to open shell token");

                if (!DuplicateTokenEx(shellToken, TOKEN_ALL_ACCESS, IntPtr.Zero, SecurityImpersonation, TokenPrimary, out IntPtr duplicatedToken))
                    throw new Exception("Failed to duplicate token");

                if (!CreateEnvironmentBlock(out IntPtr envBlock, duplicatedToken, false))
                    throw new Exception("Failed to create environment block");

                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES
                {
                    nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES)),
                    bInheritHandle = true,
                    lpSecurityDescriptor = IntPtr.Zero
                };

                logHandle = CreateFile(
                    logFilePath,
                    GENERIC_WRITE,
                    FILE_SHARE_READ,
                    ref sa,
                    CREATE_ALWAYS,
                    FILE_ATTRIBUTE_NORMAL,
                    IntPtr.Zero);

                if (logHandle == IntPtr.Zero || logHandle == new IntPtr(-1))
                    throw new Exception("Failed to create inheritable log file handle");

                STARTUPINFO si = new()
                {
                    cb = Marshal.SizeOf(typeof(STARTUPINFO)),
                    lpDesktop = @"winsta0\default",
                    dwFlags = STARTF_USESTDHANDLES,
                    hStdOutput = logHandle,
                    hStdError = logHandle
                };

                string commandLine = $"\"{exePath}\" {args}";

                EnablePrivilege("SeAssignPrimaryTokenPrivilege");
                EnablePrivilege("SeIncreaseQuotaPrivilege");

                bool result = CreateProcessAsUser(
                    duplicatedToken, null, commandLine, IntPtr.Zero, IntPtr.Zero, true,
                    CREATE_UNICODE_ENVIRONMENT | CREATE_NEW_CONSOLE, envBlock, workingDir, ref si, out pi);

                if (!result)
                {
                    int err = Marshal.GetLastWin32Error();
                    return false;
                }

                // Wait for process to exit before reading log
                Process launchedProcess = Process.GetProcessById((int)pi.dwProcessId);
                launchedProcess.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                // Avoid writing to log file while it's open
                try
                {
                    System.IO.File.AppendAllText(logFilePath, $"Error launching non-admin process: {ex.Message}\n");
                }
                catch { /* Ignore secondary errors */ }

                return false;
            }
            finally
            {
                if (logHandle != IntPtr.Zero && logHandle != new IntPtr(-1))
                    CloseHandle(logHandle);
                if (pi.hProcess != IntPtr.Zero) CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero) CloseHandle(pi.hThread);
                shellProcess?.Dispose();
            }
        }
    }
}
