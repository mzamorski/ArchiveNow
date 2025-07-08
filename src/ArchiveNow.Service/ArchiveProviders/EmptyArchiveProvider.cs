﻿using System;
using ArchiveNow.Providers.Core;

namespace ArchiveNow.Service.ArchiveProviders
{
    public class EmptyArchiveProvider : IArchiveProvider
    {
        private static readonly Lazy<EmptyArchiveProvider> _instance = new Lazy<EmptyArchiveProvider>(() => new EmptyArchiveProvider());

        public static EmptyArchiveProvider Instance => _instance.Value;

        public void Dispose()
        { }

        public string FileExtension => string.Empty;

        public string Password => string.Empty;

        public string ArchiveFilePath => string.Empty;

        public void AddDirectory(string path)
        { }

        public void Add(string path)
        { }

        public void BeginUpdate()
        { }

        public void CommitUpdate()
        { }

        public void AbortUpdate()
        { }

        public event EventHandler FileCompressionStarted;
        public event EventHandler FileCompressed;
        public event EventHandler Finished;
        public event EventHandler<int> Starting;
        public event EventHandler<bool> IsProgressIndeterminateChanged;
    }
}
