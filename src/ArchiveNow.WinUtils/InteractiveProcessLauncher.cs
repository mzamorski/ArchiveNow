using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ArchiveNow.WinUtils
{
    /// <summary>
    /// Launches processes in the interactive user session from a Windows service (LocalSystem).
    /// Wraps CreateProcessAsUser and related Win32 APIs.
    /// </summary>
    public static class InteractiveProcessLauncher
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint SessionId, out IntPtr Token);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars, dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput, hStdOutput, hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        private const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        /// <summary>
        /// Launches an executable in the currently active user session.
        /// </summary>
        /// <param name="exePath">Full path to the EXE file.</param>
        /// <param name="arguments">Command-line arguments.</param>
        /// <param name="workingDir">Working directory (defaults to EXE directory).</param>
        /// <returns>True if the process was launched successfully.</returns>
        public static bool LaunchInActiveSession(string exePath, string arguments, string workingDir = null)
        {
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                return false;

            uint sessionId = WTSGetActiveConsoleSessionId();
            if (sessionId == 0xFFFFFFFF)
                return false;

            IntPtr userToken;
            if (!WTSQueryUserToken(sessionId, out userToken) || userToken == IntPtr.Zero)
                return false;

            IntPtr env = IntPtr.Zero;
            try
            {
                if (!CreateEnvironmentBlock(out env, userToken, false))
                    env = IntPtr.Zero;

                var si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.lpDesktop = @"winsta0\default";

                var pi = new PROCESS_INFORMATION();
                string cmd = "\"" + exePath + "\" " + (arguments ?? string.Empty);

                bool ok = CreateProcessAsUser(
                    userToken,
                    null,
                    cmd,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    CREATE_UNICODE_ENVIRONMENT,
                    env,
                    workingDir ?? Path.GetDirectoryName(exePath),
                    ref si,
                    out pi);

                if (ok)
                {
                    if (pi.hThread != IntPtr.Zero) CloseHandle(pi.hThread);
                    if (pi.hProcess != IntPtr.Zero) CloseHandle(pi.hProcess);
                }

                return ok;
            }
            finally
            {
                if (env != IntPtr.Zero) DestroyEnvironmentBlock(env);
                if (userToken != IntPtr.Zero) CloseHandle(userToken);
            }
        }
    }
}
