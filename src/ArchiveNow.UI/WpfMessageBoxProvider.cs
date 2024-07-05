using System.Windows;

namespace ArchiveNow.UI
{
    public class WpfMessageBoxProvider : IMessageBoxProvider
    {
        public void ShowError(string message)
        {
            ShowError(message, "Error");
        }

        public void ShowError(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}