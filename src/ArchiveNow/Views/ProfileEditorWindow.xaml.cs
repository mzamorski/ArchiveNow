using System.Windows;

namespace ArchiveNow.Views
{
    /// <summary>
    /// Interaction logic for ProfileEditorWindow.xaml
    /// </summary>
    public partial class ProfileEditorWindow : Window
    {
        public ProfileEditorWindow()
        {
            InitializeComponent();
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
