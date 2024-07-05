using System.Collections.ObjectModel;

namespace ArchiveNow.Views.ViewModels
{
    public class ArchiveNowProfileViewModel
    {
        public string Name { get; set; }

        public string FileNameBuilder { get; set; }

        public ObservableCollection<ArchiveNowProfileIgnoreEntryViewModel> IgnoreEntries { get; set; }
    }

    public class ArchiveNowProfileIgnoreEntryViewModel
    {
        public ArchiveNowProfileIgnoreEntryViewModel(string pattern, IgnoreEntryType entryType)
            : this()
        {
            EntryType = entryType;
            Pattern = pattern;
        }

        public ArchiveNowProfileIgnoreEntryViewModel()
        { }

        public IgnoreEntryType EntryType { get; set; }

        public string Pattern { get; set; }
    }

    public enum IgnoreEntryType
    {
        File, Directory
    }
}
