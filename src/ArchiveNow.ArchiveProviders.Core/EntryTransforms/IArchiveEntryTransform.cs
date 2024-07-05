namespace ArchiveNow.Providers.Core.EntryTransforms
{
    public interface IArchiveEntryTransform
    {
        string RootPath { get; }

        string Transform(string path);
    }
}
