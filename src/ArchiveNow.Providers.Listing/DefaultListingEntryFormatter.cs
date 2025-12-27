using System;

namespace ArchiveNow.Providers.Listing
{
    internal class DefaultListingEntryFormatter : IListingEntryFormatter
    {
        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        private const string Separator = "|";

        public static DefaultListingEntryFormatter Instance { get; } = new Lazy<DefaultListingEntryFormatter>(() => new DefaultListingEntryFormatter()).Value;

        public DefaultListingEntryFormatter()
        { }

        public string Format(ListingEntry entry)
        {
            return $"{entry.Path}{Separator}{entry.Size}{Separator}{entry.ModifiedDate.ToString(DateTimeFormat)}{Separator}{entry.Hash}";
        }
    }
}
