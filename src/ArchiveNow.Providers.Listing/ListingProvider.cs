using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Listing.FailedHashProviders;
using ArchiveNow.Providers.Listing.HashFormatters;

namespace ArchiveNow.Providers.Listing
{
    public class ListingProvider : ArchiveProviderBase, IDisposable
    {
        private readonly IArchiveEntryTransform _entryTransform;
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly List<ListingEntry> _entries = new List<ListingEntry>();
        private readonly IListingEntryFormatter _lineFormatter;
        private readonly IHashFormatter _hashFormatter = new DefaultHashFormatter();
        private readonly IFailedHashProvider _failedHashProvider;
        private bool _disposed;
        private readonly bool _useParallelProcessing;

        public override string FileExtension => "lst";

        public ListingProvider(
            IArchiveFilePathBuilder pathBuilder,
            IArchiveEntryTransform entryTransform,
            bool useParallelProcessing)
            : base(pathBuilder)
        {
            SimulateLatency = true;

            _entryTransform = entryTransform;
            _useParallelProcessing = useParallelProcessing;

            _failedHashProvider = new MessageFailedHashProvider();
            _lineFormatter = DefaultListingEntryFormatter.Instance;

            // Create instance only if we are running sequentially.
            // In parallel mode, instances are created inside the parallel loop to ensure thread safety without locking.
            if (!_useParallelProcessing)
            {
                _hashAlgorithm = MD5.Create();
            }
        }

        public override void Add(string path)
        {
            var fileInfo = new FileInfo(path);

            var entry = new ListingEntry(path)
            {
                Size = fileInfo.Length,
                ModifiedDate = fileInfo.LastWriteTime
            };

            if (_useParallelProcessing)
            {
                //ApplySimulateLatency(1);
            }
            else
            {
                // Sequential mode: calculate hash immediately using the shared instance.
                entry.Hash = CalculateHash(path, _hashAlgorithm);
            }

            _entries.Add(entry);
        }

        public override void AddDirectory(string path)
        { }

        public override void BeginUpdate(string sourcePath)
        { 
            CurrentProgressMode = (_useParallelProcessing) ? ProgressMode.Indeterminate : ProgressMode.Determinate; 
        }

        public override void CommitUpdate()
        {
            // If parallel mode is enabled, compute hashes now (batch processing).
            if (_useParallelProcessing)
            {
                ComputeHashesParallel();
            }

            // Write result to the output file.
            using (var writer = new StreamWriter(ArchiveFilePath))
            {
                foreach (var entry in _entries)
                {
                    writer.WriteLine(_lineFormatter.Format(entry));
                }
            }
        }

        public override void AbortUpdate()
        {
            _entries.Clear();
        }

        private string CalculateHash(string filePath, HashAlgorithm algorithm)
        {
            try
            {
                using (var stream = OpenFile(filePath))
                {
                    byte[] hash = algorithm.ComputeHash(stream);
                    return _hashFormatter.Format(hash);
                }
            }
            catch
            {
                return _failedHashProvider.Get();
            }
        }

        private void ComputeHashesParallel()
        {
            // Optimization: Hash instance is created once per thread, not once per file.
            // This reduces object allocation and GC pressure.
            Parallel.ForEach(
                _entries,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                () => MD5.Create(), // Thread-local init
                (item, loopState, algorithm) =>
                {
                    item.Hash = CalculateHash(item.Path, algorithm);

                    return algorithm;
                },
                (algorithm) => algorithm.Dispose() // Thread-local cleanup
            );
        }

        private Stream OpenFile(string filePath)
        {
            return new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Only dispose _hashAlgorithm if it was created (sequential mode).
                _hashAlgorithm?.Dispose();

                _entries.Clear();
            }

            _disposed = true;
        }
    }
}
