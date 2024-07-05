using System;
using System.Collections.Generic;

using ArchiveNow.Actions.Core;
using ArchiveNow.Providers.Core.FileNameBuilders;

namespace ArchiveNow.Configuration.Profiles
{
    public class NullArchiveNowProfile : IArchiveNowProfile
    {
        private const string EmptyValue = "Empty (null)";

        private static readonly Lazy<NullArchiveNowProfile> _instance =
            new Lazy<NullArchiveNowProfile>(() => new NullArchiveNowProfile());

        public static IArchiveNowProfile Instance => _instance.Value;

        protected NullArchiveNowProfile()
        { }

        public virtual string Name => EmptyValue;

        public ISet<string> IgnoredDirectories => new HashSet<string>();

        public ISet<string> IgnoredFiles => new HashSet<string>();

        public IFileNameBuilder FileNameBuilder => NullFileNameBuilder.Instance;

        public bool IsEmpty => true;

        public string Location
        {
            get => string.Empty;
            set { }
        }

        public string Password => string.Empty;

        public bool UsePlainTextPasswords => false;

        public string ProviderName => string.Empty;

        public bool UseDefaultActionPrecedence => true;

        public IList<IAfterFinishedAction> AfterFinishedActions => new List<IAfterFinishedAction>(0);

        public DateTime CreateDate
        {
            get => DateTime.MinValue;

            set
            { }
        }

        public DateTime ModifyDate
        {
            get => DateTime.MinValue;

            set
            { }
        }


        public string DestinationPath => string.Empty;

        public bool BreakActionsIfError => false;

        bool IArchiveNowProfile.UsePlainTextPasswords { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual bool IsValid(out string message)
        {
            message = string.Empty;
            return true;
        }
    }
}