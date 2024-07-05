using System.IO;
using System.Linq;

namespace ArchiveNow.Utils.IO
{
    public static class FileSystemExtensions
    {
        public const uint INVALID_FILE_ATTRIBUTES = uint.MaxValue;

        public static bool IsValid(this FileAttributes attributes)
            => attributes != unchecked((FileAttributes)INVALID_FILE_ATTRIBUTES);

        public static bool IsDirectory(this FileAttributes attributes)
            => attributes.IsValid() && attributes.HasFlag(FileAttributes.Directory);

        public static bool IsPathTooLong(this FileSystemInfo info)
        {
            return PathHelper.IsTooLong(info.FullName);
        }

        public static bool IsDirectory(this FileSystemInfo info)
        {
            return (info is DirectoryInfo);
        }

        public static bool IsFile(this FileSystemInfo info)
        {
            return (info is FileInfo);
        }

        public static bool IsDirectory(this string path)
        {
            return Directory.Exists(path);
        }

        public static bool IsDirectoryEmpty(this string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public static bool PathExists(this string path)
        {
            if (path == null)
            {
                return false;
            }

            return (Directory.Exists(path) || File.Exists(path));
        }
    }
}
