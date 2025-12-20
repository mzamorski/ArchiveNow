using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Listing.FailedHashProviders;
using ArchiveNow.Providers.Listing.HashFormatters;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ArchiveNow.Providers.Listing
{
    public class ListingProvider : ArchiveProviderBase
    {
        private readonly IArchiveEntryTransform _entryTransform;
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly List<ListingEntry> _entries = new List<ListingEntry>();
        private readonly IListingEntryFormatter _lineFormatter;
        private readonly IHashFormatter _hashFormatter = new DefaultHashFormatter();
        private readonly IFailedHashProvider _failedHashProvider;

        public override string FileExtension => "lst";

        public ListingProvider(IArchiveFilePathBuilder pathBuilder, IArchiveEntryTransform entryTransform) 
            : base(pathBuilder)
        {
            _entryTransform = entryTransform;
            _hashAlgorithm = MD5.Create();
            _failedHashProvider = new MessageFailedHashProvider();
            _lineFormatter = DefaultListingEntryFormatter.Instance;
        }

        public override void Add(string path)
        {
            var fileInfo = new FileInfo(path);

            var hash = CalculateHash(path);

            _entries.Add(new ListingEntry(_entryTransform.Transform(path))
            {
                Size = fileInfo.Length,
                ModifiedDate = fileInfo.LastWriteTime,
                Hash = hash
            });
        }

        public override void AddDirectory(string path)
        { }

        public override void BeginUpdate(string sourcePath)
        {
        }

        public override void CommitUpdate()
        {
            using (var writer = new StreamWriter(ArchiveFilePath))
            {
                foreach (var entry in _entries)
                {
                    writer.WriteLine(_lineFormatter.Format(entry));
                }
            }

            _hashAlgorithm?.Dispose();
        }

        public override void AbortUpdate()
        {
            _hashAlgorithm?.Dispose();
        }

        public string CalculateHash(string filePath)
        {
            try
            {
                using (var stream = OpenFile(filePath))
                {
                    byte[] hash = _hashAlgorithm.ComputeHash(stream);
                    return _hashFormatter.Format(hash);
                }
            }
            catch
            {
                return _failedHashProvider.Get();
            }
        }

        private Stream OpenFile(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }
    }
}
