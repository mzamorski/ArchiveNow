using System;
using System.Collections.Generic;

using ArchiveNow.Actions.Core;
using ArchiveNow.Providers.Core.FileNameBuilders;

namespace ArchiveNow.Configuration.Profiles
{
    public interface IArchiveNowProfile
    {
        string Name { get; }

        ISet<string> IgnoredDirectories { get; }

        ISet<string> IgnoredFiles { get; }

        IFileNameBuilder FileNameBuilder { get; }

        bool IsEmpty { get; }

        string Location { get; set; }

        string Password { get; }

        bool UsePlainTextPasswords { get; set; }

        string ProviderName { get; }

        bool UseDefaultActionPrecedence { get; }

        bool BreakActionsIfError { get; }

        IList<IAfterFinishedAction> AfterFinishedActions { get; }

        DateTime CreateDate { get; set; }

        DateTime ModifyDate { get; set; }

        bool IsValid(out string message);
    }
}