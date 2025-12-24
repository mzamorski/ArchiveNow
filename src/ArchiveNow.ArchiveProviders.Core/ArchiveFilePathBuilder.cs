using System.IO;

using ArchiveNow.Core.Loggers;
using ArchiveNow.Utils;
using ArchiveNow.Utils.Exceptions;
using ArchiveNow.Utils.IO;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class ArchiveFilePathBuilder : ArchiveFilePathBuilderBase
    {
        private const string DirectorySourceLabel = "directory";
        private const string FileSourceLabel = "file";

        private readonly IFileNameBuilderContext _context;
        private readonly IArchiveNowLogger _logger;
        private readonly FileInfo _sourcePath;
        private readonly IFileNameBuilder _fileNameBuilder;

        public override IFileNameBuilderContext Context => _context;

        public ArchiveFilePathBuilder(string sourcePath, IFileNameBuilder fileNameBuilder)
        {
            _sourcePath = new FileInfo(sourcePath);
            _fileNameBuilder = fileNameBuilder;
        }

        public ArchiveFilePathBuilder(
            string sourcePath,
            IFileNameBuilder fileNameBuilder,
            IFileNameBuilderContext context,
            IArchiveNowLogger logger)

            : this(sourcePath, fileNameBuilder)
        {
            _context = context;
            _logger = logger;
        }

        public override string Build(string fileExtension)
        {
            var parentDirectory = _sourcePath.Directory;
            var sourceTypeName = (_sourcePath.IsDirectory() ? DirectorySourceLabel : FileSourceLabel);

            _logger.Debug($"Path: {_sourcePath.FullName} (is a {sourceTypeName}); Parent: {parentDirectory}");

            var basePath = parentDirectory.FullName;
            var path = Path.Combine(basePath, _fileNameBuilder.Build(_context));

            var archiveFilePath = ChangeExtension(path, fileExtension);

            if (archiveFilePath.IsEmpty())
            {
                throw new InvalidPathException($"The specified path is invalid: {archiveFilePath}");
            }

            return archiveFilePath;
        }
    }
}