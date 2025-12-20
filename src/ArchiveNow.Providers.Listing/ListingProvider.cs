using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Listing.FailedHashProviders;
using ArchiveNow.Providers.Listing.HashFormatters;

namespace ArchiveNow.Providers.Listing
{
    public class ListingProvider : ArchiveProviderBase
    {
        private readonly string ErrorHash = "<Failed to compute hash>";

        private readonly HashAlgorithm _hashAlgorithm;
        private readonly List<ListingEntry> _entries = new List<ListingEntry>();
        private readonly IListingEntryFormatter _lineFormatter;
        private readonly IHashFormatter _hashFormatter = new DefaultHashFormatter();
        private readonly IFailedHashProvider _failedHashProvider;

        public override string FileExtension => "lst";

        public ListingProvider(IArchiveFilePathBuilder pathBuilder) 
            : base(pathBuilder)
        {
            _hashAlgorithm = MD5.Create();
            _failedHashProvider = new MessageFailedHashProvider();
            _lineFormatter = DefaultListingEntryFormatter.Instance;
        }

        public override void Add(string path)
        {
            var fileInfo = new FileInfo(path);

            var hash = CalculateHash(path);

            _entries.Add(new ListingEntry(path)
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
