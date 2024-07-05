using System;
using System.Collections.Generic;

using ArchiveNow.Actions.Core;
using ArchiveNow.Providers.Core.FileNameBuilders;

namespace ArchiveNow.Configuration.Profiles
{
    public class ArchiveNowProfile : IArchiveNowProfile
    {
        public bool IsEmpty => false;

        /// <summary>
        /// Profile name.
        /// </summary>
        public string Name { get; set; }

        public string Location { get; set; }

        public string Password { get; set; }

        public bool UsePlainTextPasswords { get; set; }

        public string ProviderName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public bool UseDefaultActionPrecedence { get; set; }

        /// <summary>
        /// Contains a list of ignored directories. It supports regular expressions.
        /// </summary>
        public ISet<string> IgnoredDirectories { get; set; } = new HashSet<string>();

        /// <summary>
        /// Contains a list of ignored directories. It supports regular expressions.
        /// </summary>
        public ISet<string> IgnoredFiles { get; set;  } = new HashSet<string>();

        public IFileNameBuilder FileNameBuilder { get; set; }

        public bool BreakActionsIfError { get; set; }

        //public string DestinationPath { get; set; }

        //public string AsymmetricPrivateKeyPath { get; set; }

        /// <summary>
        /// Actions that will be called after the compression ends. 
        /// </summary>
        public IList<IAfterFinishedAction> AfterFinishedActions { get; set; } = new List<IAfterFinishedAction>();

        public ArchiveNowProfile()
        {
            UseDefaultActionPrecedence = true;
        }

        public bool IsValid(out string message)
        {
            //if (DestinationPath.HasValue())
            //{
            //    if (!Directory.Exists(DestinationPath))
            //    {
            //        message = "The destination path ([DestinationPath]) is set, but the folder does not exist!";
            //        return false;
            //    }
            //}

            message = string.Empty;

            return true;
        }

        public override string ToString()
        {
            return $"Name: `{Name}`\nProviderName: `{ProviderName}`";
        }
    }
}
