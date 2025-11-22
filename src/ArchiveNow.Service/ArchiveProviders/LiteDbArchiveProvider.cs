using System;
using System.Collections.Generic;
using System.IO;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.PasswordProviders;
using LiteDB;

namespace ArchiveNow.Service.ArchiveProviders
{
    public class LiteDbArchiveProvider : ArchiveProviderBase
    {
        private readonly IArchiveEntryTransform _entryTransform;

        public LiteDbArchiveProvider(IArchiveFilePathBuilder pathBuilder, IArchiveEntryTransform entryTransform, IPasswordProvider passwordProvider) 
            : base(pathBuilder, passwordProvider)
        {
            _entryTransform = entryTransform;
        }

        public override string FileExtension => "ldb";

        public override void AddDirectory(string path)
        {
            var id = Guid.NewGuid().ToString("N");

            Database.FileStorage.Upload(id, path, Stream.Null);

            var directoryPath = Directory.GetParent(path).Name;
            var relativePath = _entryTransform.Transform(directoryPath);

            var metadata = new BsonDocument(new Dictionary<string, BsonValue>
            {
                {"parentDirectory", relativePath},
                {"isDirectory", true}
            });
            Database.FileStorage.SetMetadata(id, metadata);
        }

        public override void Add(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            var relativePath = _entryTransform.Transform(directoryPath);

            var id = Guid.NewGuid().ToString("N");

            Database.FileStorage.Upload(id, path);

            var metadata = new BsonDocument(new Dictionary<string, BsonValue> {{"parentDirectory", relativePath}});
            Database.FileStorage.SetMetadata(id, metadata);
        }

        public override void BeginUpdate(string sourcePath)
        {
            var connectionString = new ConnectionString(ArchiveFilePath)
            {
                Password = base.Password,
            };

            Database = new LiteDatabase(connectionString);
        }

        private LiteDatabase Database { get; set; }

        public override void CommitUpdate()
        {
            //Database.Shrink();
            Database.Dispose();
        }

        public override void AbortUpdate()
        {
            Database.Dispose();
        }
    }
}