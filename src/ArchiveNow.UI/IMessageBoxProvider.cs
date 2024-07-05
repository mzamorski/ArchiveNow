namespace ArchiveNow.UI
{
    public interface IMessageBoxProvider
    {
        void ShowError(string message);
        void ShowError(string message, string caption);
    }
}
