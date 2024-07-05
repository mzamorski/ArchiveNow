using System;
using System.IO;
using ArchiveNow.Configuration.Storages;

namespace ArchiveNow.Providers.Core.FileNameBuilders
{
    public class AddVersionFileNameBuilder : IFileNameBuilder
    {
        private const string PreferenceName = "Version";

        public IStorage Storage { get; }

        public AddVersionFileNameBuilder(IStorage storage)
        {
            Storage = storage;
        }

        public string Build(IFileNameBuilderContext context)
        {
            var fileName = Path.GetFileName(context.SourcePath);

            var keyName = $"{PreferenceName}\\{fileName}";
            if (!Storage.Exists(keyName))
            {
                Storage.Store(keyName, "1.0");
            }

            var number = Storage.Get(keyName);
            var currentVersion = new Version(number);

            return $"{fileName}-{currentVersion.ToString(2)}";
        }
    }

    public class RenameFileNameBuilder : IFileNameBuilder
    {
        public string Name { get; } = "Enshrouded";

        string IFileNameBuilder.Build(IFileNameBuilderContext context)
        {
            return Name;
        }
    }
}
