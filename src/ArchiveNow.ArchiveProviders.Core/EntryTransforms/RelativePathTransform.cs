using System;
using System.IO;

namespace ArchiveNow.Providers.Core.EntryTransforms
{
    public class RelativePathTransform : IArchiveEntryTransform
    {
        private readonly bool _forceForwardSlashes;
        private readonly bool _appendTrailingSlashToDirectories;

        public string RootPath { get; }

        /// <summary>
        /// Initializes path transformer.
        /// </summary>
        /// <param name="sourcePath">The base root path.</param>
        /// <param name="forceForwardSlashes">If true, forces '/' separator (required for TAR/ZIP formats).</param>
        /// <param name="appendTrailingSlashToDirectories">If true, appends a trailing slash if the path identifies a directory.</param>
        public RelativePathTransform(string sourcePath, bool forceForwardSlashes = false, bool appendTrailingSlashToDirectories = false)
        {
            RootPath = sourcePath;

            _forceForwardSlashes = forceForwardSlashes;
            _appendTrailingSlashToDirectories = appendTrailingSlashToDirectories;

            // Ensure RootPath ends with a separator for correct Uri processing
            if (!RootPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !RootPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                RootPath += Path.DirectorySeparatorChar;
            }
        }

        public string Transform(string path)
        {
            // Directory Handling
            // --
            // If enabled, check if the path is a physical directory.
            // We append a separator temporarily so Uri.MakeRelativeUri recognizes it as a directory
            // and preserves the trailing slash in the output.
            if (_appendTrailingSlashToDirectories)
            {
                if (Directory.Exists(path))
                {
                    if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                        !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                    {
                        path += Path.DirectorySeparatorChar;
                    }
                }
            }

            var rootUri = new Uri(RootPath);
            var pathUri = new Uri(path);

            // Calculate relative URI
            Uri relativeUri = rootUri.MakeRelativeUri(pathUri);

            // Decode special characters (e.g., spaces replaced by %20)
            string resultPath = Uri.UnescapeDataString(relativeUri.ToString());

            // Separator Normalization
            if (_forceForwardSlashes)
            {
                // For e.g. TAR: force forward slashes regardless of the OS
                resultPath = resultPath.Replace('\\', '/');
            }
            else
            {
                // Default: use system separator (e.g., backslash on Windows)
                resultPath = resultPath.Replace('/', Path.DirectorySeparatorChar);
            }

            return resultPath;
        }

        /// <summary>
        /// Creates an instance configured for TAR format (forward slashes, trailing separator for directories).
        /// </summary>
        /// <param name="sourcePath">The root path to calculate relative paths from.</param>
        /// <returns>A RelativePathTransform configured for TAR archives.</returns>
        public static IArchiveEntryTransform ForTarFormat(string sourcePath)
        {
            return new RelativePathTransform(sourcePath, true, true);
        }

        /// <summary>
        /// Creates an instance configured for ZIP format (system separator, no trailing separator).
        /// </summary>
        /// <param name="sourcePath">The root path to calculate relative paths from.</param>
        /// <returns>A RelativePathTransform configured for ZIP archives.</returns>
        public static IArchiveEntryTransform ForZipFormat(string sourcePath)
        {
            return new RelativePathTransform(sourcePath, false, false);
        }
    }

    ///// <summary>
    ///// Transforms absolute paths to relative paths with configurable separator and directory formatting.
    ///// </summary>
    //public class RelativePathTransform : IArchiveEntryTransform
    //{
    //    private readonly char _outputSeparator;
    //    private readonly bool _appendSeparatorToDirectories;

    //    public string RootPath { get; }

    //    /// <summary>
    //    /// Initializes a new instance with default settings (system separator, no trailing separator).
    //    /// </summary>
    //    /// <param name="sourcePath">The root path to calculate relative paths from.</param>
    //    public RelativePathTransform(string sourcePath)
    //        : this(sourcePath, Path.DirectorySeparatorChar, false)
    //    {
    //    }

    //    /// <summary>
    //    /// Initializes a new instance with custom separator and directory formatting.
    //    /// </summary>
    //    /// <param name="sourcePath">The root path to calculate relative paths from.</param>
    //    /// <param name="outputSeparator">The separator to use in output paths (e.g., '/' for TAR format).</param>
    //    /// <param name="appendSeparatorToDirectories">Whether to append separator to directory paths.</param>
    //    public RelativePathTransform(string sourcePath, char outputSeparator, bool appendSeparatorToDirectories)
    //    {
    //        if (string.IsNullOrWhiteSpace(sourcePath))
    //        {
    //            throw new ArgumentNullException(nameof(sourcePath));
    //        }

    //        RootPath = Path.GetFullPath(sourcePath);

    //        _outputSeparator = outputSeparator;
    //        _appendSeparatorToDirectories = appendSeparatorToDirectories;
    //    }

    //    /// <summary>
    //    /// Creates an instance configured for TAR format (forward slashes, trailing separator for directories).
    //    /// </summary>
    //    /// <param name="sourcePath">The root path to calculate relative paths from.</param>
    //    /// <returns>A RelativePathTransform configured for TAR archives.</returns>
    //    public static IArchiveEntryTransform ForTarFormat(string sourcePath)
    //    {
    //        return new RelativePathTransform(sourcePath, '/', true);
    //    }

    //    /// <summary>
    //    /// Creates an instance configured for ZIP format (system separator, no trailing separator).
    //    /// </summary>
    //    /// <param name="sourcePath">The root path to calculate relative paths from.</param>
    //    /// <returns>A RelativePathTransform configured for ZIP archives.</returns>
    //    public static IArchiveEntryTransform ForZipFormat(string sourcePath)
    //    {
    //        return new RelativePathTransform(sourcePath, Path.DirectorySeparatorChar, false);
    //    }

    //    public string Transform(string path)
    //    {
    //        if (string.IsNullOrWhiteSpace(path))
    //        {
    //            throw new ArgumentNullException(nameof(path));
    //        }

    //        var fullPath = Path.GetFullPath(path);
    //        var normalizedRoot = RootPath;

    //        // Ensure root ends with separator for proper comparison
    //        if (!normalizedRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
    //        {
    //            normalizedRoot += Path.DirectorySeparatorChar;
    //        }

    //        // Check if path is under root
    //        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
    //        {
    //            throw new ArgumentException($"Path '{path}' is not under root path '{RootPath}'");
    //        }

    //        // Extract relative path
    //        var relativePath = fullPath.Substring(normalizedRoot.Length);

    //        // Replace separators if needed
    //        if (_outputSeparator != Path.DirectorySeparatorChar)
    //        {
    //            relativePath = relativePath.Replace(Path.DirectorySeparatorChar, _outputSeparator);
    //        }
    //        if (_outputSeparator != Path.AltDirectorySeparatorChar)
    //        {
    //            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, _outputSeparator);
    //        }

    //        // Append separator to directories if enabled
    //        if (_appendSeparatorToDirectories && Directory.Exists(fullPath))
    //        {
    //            if (!relativePath.EndsWith(_outputSeparator.ToString()))
    //            {
    //                relativePath += _outputSeparator;
    //            }
    //        }

    //        return relativePath;
    //    }
    //}

    ////public class RelativePathTransform : IArchiveEntryTransform
    ////{
    ////    public string RootPath { get; }

    ////    public RelativePathTransform(string sourcePath)
    ////    {
    ////        RootPath = sourcePath;
    ////    }

    ////    public string Transform(string path)
    ////    {
    ////        //return path.Replace(RootPath, string.Empty);

    ////        var pathUri = new Uri(path);
    ////        var directoryPath = RootPath;

    ////        // Folders must end in a slash
    ////        if (!directoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
    ////        {
    ////            directoryPath += Path.DirectorySeparatorChar;
    ////        }
    ////        var directoryUri = new Uri(directoryPath);

    ////        return Uri.UnescapeDataString(directoryUri.MakeRelativeUri(pathUri).ToString()
    ////            .Replace('/', Path.DirectorySeparatorChar));
    ////    }
    ////}
}
