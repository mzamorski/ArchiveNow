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

        /// <summary>
        /// Determines if the path is a file or directory and deletes it.
        /// Returns true if the element existed and deletion was attempted.
        /// Returns false if the element did not exist.
        /// </summary>
        public static bool DeletePath(string path)
        {
            try
            {
                // Optimization: GetAttributes checks existence and type in a single I/O call.
                // If the path does not exist, it throws FileNotFoundException or DirectoryNotFoundException.
                FileAttributes attr = File.GetAttributes(path);

                // Check if the path points to a directory
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    // Recursive delete allows removing non-empty directories
                    Directory.Delete(path, true);
                }
                else
                {
                    // Remove ReadOnly attribute if present to prevent UnauthorizedAccessException on delete
                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(path, attr & ~FileAttributes.ReadOnly);
                    }

                    File.Delete(path);
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                return false; // File does not exist
            }
            catch (DirectoryNotFoundException)
            {
                return false; // Directory does not exist
            }
            // Other exceptions (e.g. UnauthorizedAccessException due to permissions) are propagated up to Execute
        }
    }
}
