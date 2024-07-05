using System;
using System.Collections.Generic;

namespace ArchiveNow.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class ArchiveNowProfile : IArchiveNowProfile
    {
        private string _name;

        /// <summary>
        /// Profile name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name ?? (this._name = RandomFileNameBuilder.Instance.Build());
            }

            set { this._name = value; }
        }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// Contains a list of ignored directories. It supports regular expressions.
        /// </summary>
        public ISet<string> IgnoredDirectories { get; } = new HashSet<string>();

        /// <summary>
        /// Contains a list of ignored directories. It supports regular expressions.
        /// </summary>
        public ISet<string> IgnoredFiles { get; } = new HashSet<string>();

        /// <summary>
        /// 
        /// </summary>
        public IFileNameBuilder FileNameBuilder { get; set; }
    }
}
