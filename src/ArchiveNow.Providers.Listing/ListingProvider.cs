using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.FileNameBuilders.Formatters;

namespace ArchiveNow.Providers.Listing
{
    /// <summary>
    /// TODO: Aby przyśpieszyć wyliczanie sum kontrolnych, można by pomyśleć jak zrównoleglić przetwarzanie w `ArchiveNowService`.
    /// </summary>
    public class ListingProvider : ArchiveProviderBase
    {
        private readonly string ErrorHash = "<Failed to compute hash>";

        private readonly HashAlgorithm _hashAlgorithm;
        private readonly List<ListingEntry> _entries = new List<ListingEntry>();
        private readonly IListingEntryFormatter _lineFormatter;
        private readonly IHashFormatter _hashFormatter = new DefaultHashFormatter();
        private readonly IErrorHashBuilder _errorHashFormatter;

        public override string FileExtension => "lst";

        public ListingProvider(IArchiveFilePathBuilder pathBuilder) 
            : base(pathBuilder)
        {
            _hashAlgorithm = MD5.Create();
            _errorHashFormatter = new DefaultErrorHashBuilder();
            //_errorHashFormatter = new ZeroErrorHashBuilder(_hashAlgorithm.HashSize);


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
                return _errorHashFormatter.Build();
            }
        }

        private Stream OpenFile(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }
    }

    internal interface IHashFormatter
    {
        string Format(byte[] hashBytes);
    }

    internal interface IErrorHashBuilder
    {
        string Build();
    }

    internal class DefaultHashFormatter : IHashFormatter
    {
        public string Format(byte[] hashBytes)
        {
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    internal class DefaultErrorHashBuilder : IErrorHashBuilder
    {
        private const string Message = "<Failed to compute hash>";
        public string Build()
        {
            return Message;
        }
    }

    internal class ZeroErrorHashBuilder : IErrorHashBuilder
    {
        private readonly int _hashCharSize;
        private readonly byte[] _zeroHashBytes;
        private readonly IHashFormatter _hashFormatter = new DefaultHashFormatter();

        public ZeroErrorHashBuilder(int hashSize)
        {
            int hashSizeInBytes = hashSize / 8;

            _zeroHashBytes = new byte[hashSizeInBytes];
            _hashCharSize = hashSize / 8;
        }

        public string Build()
        {
            return _hashFormatter.Format(_zeroHashBytes);
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
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        private const string Separator = "|";

        public static DefaultListingEntryFormatter Instance { get; } = new Lazy<DefaultListingEntryFormatter>(() => new DefaultListingEntryFormatter()).Value;

        public string Format(ListingEntry entry)
        {
            return $"{entry.Path}{Separator}{entry.Size}{Separator}{entry.ModifiedDate.ToString(DateTimeFormat)}{Separator}{entry.Hash}";
        }
    }
}
