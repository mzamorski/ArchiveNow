﻿using System;

namespace ArchiveNow.Service
{
    public class ArchiveResult : IArchiveResult
    {
        public bool IsSuccess { get; }
        
        public string Message { get; set; }

        public TimeSpan Duration { get; }

        public string ArchivePath { get; }

        public ArchiveResult(bool isSuccess)
            : this (isSuccess, TimeSpan.Zero, string.Empty)
        { }

        private ArchiveResult(bool isSuccess, TimeSpan duration, string archiveFilePath)
        {
            IsSuccess = isSuccess;
            Duration = duration;
            ArchivePath = archiveFilePath;
        }

        public static ArchiveResult Success(TimeSpan duration, string archiveFilePath)
        {
            return new ArchiveResult(true, duration, archiveFilePath);
        }

        public static ArchiveResult Fail(string message)
        {
            return new ArchiveResult(false) { Message = message };
        }
    }
}