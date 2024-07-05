using System;
using NLog;
using NLog.Targets;

namespace ArchiveNow.Core.Loggers
{
    public class FileLogger : IArchiveNowLogger
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string Name => _logger.Name;

        public string FilePath => GetLogFilePath();

        private static string GetLogFilePath()
        {
            var fileTarget = (FileTarget)LogManager.Configuration.FindTargetByName("file");

            // Need to set timestamp here if `filename` uses date. 
            // For example: filename="${basedir}/logs/${shortdate}/trace.log"
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };

            return fileTarget.FileName.Render(logEventInfo);
        }

        public void Info(string message)
        {
            var logEvent = new LogEventInfo(LogLevel.Info, _logger.Name, message);

            _logger.Log(typeof(FileLogger), logEvent);

            //_logger.Info(messageFormat, args);
        }

        public void Error(string message)
        {
            var logEvent = new LogEventInfo(LogLevel.Error, _logger.Name, message);

            _logger.Log(typeof(FileLogger), logEvent);
            //_logger.Error(message);
        }

        public void Debug(string message)
        {
            var logEvent = new LogEventInfo(LogLevel.Debug, _logger.Name, message);

            _logger.Log(typeof(FileLogger), logEvent);

            //_logger.Debug(message);
        }

        public void Warning(string message)
        {
            var logEvent = new LogEventInfo(LogLevel.Warn, _logger.Name, message);

            _logger.Log(typeof(FileLogger), logEvent);
            
            //_logger.Warn(message);
        }
    }
}