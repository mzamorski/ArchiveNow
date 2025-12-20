using System;

namespace ArchiveNow.Providers.Listing
{
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
}
