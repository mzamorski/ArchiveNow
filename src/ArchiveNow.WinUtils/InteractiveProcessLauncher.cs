using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ArchiveNow.WinUtils
{
    /// <summary>
    /// Launches processes in the interactive user session from a Windows service (LocalSystem).
    /// Wraps CreateProcessAsUser and related Win32 APIs. Target: .NET Framework 4.8.
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
            string lpApplicationName,       // can be null
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

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;    // "winsta0\\default" for interactive
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
        private const uint CREATE_NO_WINDOW = 0x08000000;
        private const int STARTF_USESHOWWINDOW = 0x00000001;
        private const short SW_HIDE = 0;

        /// <summary>
        /// Launches an executable in the currently active user session (no console window).
        /// Returns false on failure.
        /// </summary>
        public static bool LaunchInActiveSession(string exePath, string arguments, string workingDir = null)
            => LaunchInActiveSession(exePath, arguments, workingDir, hideWindow: true, out _);

        /// <summary>
        /// Launches an executable in the currently active user session (optionally hidden window).
        /// Outputs last Win32 error code if creation fails.
        /// </summary>
        public static bool LaunchInActiveSession(string exePath, string arguments, string workingDir, bool hideWindow, out int lastWin32Error)
        {
            lastWin32Error = 0;

            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                lastWin32Error = 2; // ERROR_FILE_NOT_FOUND
                return false;
            }

            uint sessionId = WTSGetActiveConsoleSessionId();
            if (sessionId == 0xFFFFFFFF) // no active session
            {
                lastWin32Error = 0;
                return false;
            }

            IntPtr userToken;
            if (!WTSQueryUserToken(sessionId, out userToken) || userToken == IntPtr.Zero)
            {
                lastWin32Error = Marshal.GetLastWin32Error();
                return false;
            }

            IntPtr env = IntPtr.Zero;
            try
            {
                // Try to create the user's environment block (not strictly required, but recommended)
                if (!CreateEnvironmentBlock(out env, userToken, false))
                {
                    env = IntPtr.Zero; // fall back to service environment
                }

                var si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.lpDesktop = @"winsta0\default";

                if (hideWindow)
                {
                    si.dwFlags |= STARTF_USESHOWWINDOW;
                    si.wShowWindow = SW_HIDE;
                }

                var pi = new PROCESS_INFORMATION();

                // When lpApplicationName is null, lpCommandLine must start with the quoted path to the exe
                string cmd = "\"" + exePath + "\" " + (arguments ?? string.Empty);
                string cwd = !string.IsNullOrEmpty(workingDir) ? workingDir : Path.GetDirectoryName(exePath);

                uint creationFlags = CREATE_UNICODE_ENVIRONMENT;
                if (hideWindow) creationFlags |= CREATE_NO_WINDOW;

                bool ok = CreateProcessAsUser(
                    userToken,
                    null,            // let Windows parse exe from the command line
                    cmd,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    creationFlags,
                    env,
                    cwd,
                    ref si,
                    out pi);

                if (!ok)
                {
                    lastWin32Error = Marshal.GetLastWin32Error();
                    return false;
                }

                // Clean up handles
                if (pi.hThread != IntPtr.Zero) CloseHandle(pi.hThread);
                if (pi.hProcess != IntPtr.Zero) CloseHandle(pi.hProcess);

                return true;
            }
            finally
            {
                if (env != IntPtr.Zero) DestroyEnvironmentBlock(env);
                if (userToken != IntPtr.Zero) CloseHandle(userToken);
            }
        }
    }
}
