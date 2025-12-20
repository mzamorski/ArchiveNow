namespace ArchiveNow.Providers.Core.EntryTransforms
{
    public class LeaveOriginalPathTransform : IArchiveEntryTransform
    {
        public string RootPath { get; }

        public LeaveOriginalPathTransform(string sourcePath)
        {
            RootPath = sourcePath;
        }

        public string Transform(string path)
        {
            return path;
        }
    }
}
