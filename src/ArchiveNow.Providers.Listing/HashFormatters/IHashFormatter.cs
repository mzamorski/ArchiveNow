namespace ArchiveNow.Providers.Listing.HashFormatters
{
    internal interface IHashFormatter
    {
        string Format(byte[] hashBytes);
    }
}
