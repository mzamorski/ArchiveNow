using System;

namespace ArchiveNow.Providers.Core.EntryTransforms
{
    public sealed class NullArchiveEntryTransform : IArchiveEntryTransform
    {
        private static readonly Lazy<NullArchiveEntryTransform> _instance = new Lazy<NullArchiveEntryTransform>(() => new NullArchiveEntryTransform());

        public static NullArchiveEntryTransform Instance => _instance.Value;

        private NullArchiveEntryTransform()
        {
            //Implent here the initialization of your singleton
        }

        public string Transform(string path)
        {
            return string.Empty;
        }

        public string RootPath => String.Empty;
    }
}
