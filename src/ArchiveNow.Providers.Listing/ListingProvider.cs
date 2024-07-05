using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using ArchiveNow.Providers.Core;

namespace ArchiveNow.Providers.Listing
{
    /// <summary>
    /// TODO: Aby przyśpieszyć wyliczanie sum kontrolnych, można by pomyśleć jak zrównoleglić przetwarzanie w `ArchiveNowService`.
    /// </summary>
    public class ListingProvider : ArchiveProviderBase
    {
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly List<ListingEntry> _entries = new List<ListingEntry>();
        private readonly IListingEntryFormatter _lineFormatter;

        public override string FileExtension => "lst";

        public ListingProvider(IArchiveFilePathBuilder pathBuilder) 
            : base(pathBuilder)
        {
            _hashAlgorithm = MD5.Create();
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

        public override void BeginUpdate()
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
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = _hashAlgorithm.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    internal class ListingEntry
    {
        public string Path { get; }

        public long Size { get; set; }
        
        public DateTime ModifiedDate { get; set; }

        public string Hash { get; set; }

        public ListingEntry(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }

    internal interface IListingEntryFormatter
    {
        string Format(ListingEntry entry);
    }

    internal class DefaultListingEntryFormatter : IListingEntryFormatter
    {
        public static DefaultListingEntryFormatter Instance { get; } = new Lazy<DefaultListingEntryFormatter>(() => new DefaultListingEntryFormatter()).Value;

        public string Format(ListingEntry entry)
        {
            return $"{entry.Path}|{entry.Size}|{entry.ModifiedDate}|{entry.Hash}";
        }
    }
}
