namespace ArchiveNow.Core.Loggers
{
    public interface IArchiveNowLogger
    {
        void Info(string messageFormat);

        void Error(string message);

        void Debug(string message);

        void Warning(string message);
    }
}
