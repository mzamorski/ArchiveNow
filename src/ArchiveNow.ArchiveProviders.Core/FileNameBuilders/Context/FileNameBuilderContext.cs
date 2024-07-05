using System;
using System.IO;

using ArchiveNow.Utils.IO;
using ArchiveNow.Utils.Exceptions;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class FileNameBuilderContext : IFileNameBuilderContext
    {
        public string SourcePath { get; }

        public string SourceDirectory
        {
            get
            {
                if (!SourcePath.PathExists())
                {
                    var path = SourcePath ?? "(null)";

                    throw new PathNotFoundException($"Could not find path '{path}'.");
                }

                return Directory.GetParent(SourcePath).FullName;
            }
        }

        public DateTime Date { get; }

        public FileNameBuilderContext(string sourcePath, DateTime date)
        {
            SourcePath = sourcePath;
            Date = date;
        }

        public FileNameBuilderContext(string fileName)
            : this(fileName, DateTime.UtcNow)
        { }
    }
}