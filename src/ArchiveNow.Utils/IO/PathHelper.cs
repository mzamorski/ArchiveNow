using System.IO;
using Microsoft.Win32;

namespace ArchiveNow.Utils.IO
{
    public static class PathHelper
    {
        private const string LONG_PATHS_ENABLED_REGISTRY_KEY_NAME =
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem";

        private const string LONG_PATHS_ENABLED_REGISTRY_VALUE_NAME = "LongPathsEnabled";

        public static bool IsLongPathsEnabled()
        {
            var longPathsEnabled = (int)Registry.GetValue(LONG_PATHS_ENABLED_REGISTRY_KEY_NAME, LONG_PATHS_ENABLED_REGISTRY_VALUE_NAME, 0);

            return longPathsEnabled != 0;
        }

        public static void SetLongPathSupport()
        {
            Registry.SetValue(LONG_PATHS_ENABLED_REGISTRY_KEY_NAME, LONG_PATHS_ENABLED_REGISTRY_VALUE_NAME, 1);
        }

        public static bool IsTooLong(string path)
        {
            try
            {
                Path.GetFullPath(path);
            }
            catch (PathTooLongException)
            {
                return true;
            }

            return false;
        }

        public static bool IsValid(this string path)
        {
            return Path.IsPathRooted(path);
        }

        public static string GetName(this string path, out string extension)
        {
            //if (!IsValid(path))
            //{
            //    throw new InvalidPathException($"The specified path '{path}' is not valid.");
            //}

            if (path.IsDirectory())
            {
                extension = string.Empty;
                return new DirectoryInfo(path).Name;
            }
            else
            {
                extension = Path.GetExtension(path);
                return Path.GetFileNameWithoutExtension(path);
            }
        }
    }
}
