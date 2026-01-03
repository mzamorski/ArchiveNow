using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Configuration.Readers;
using ArchiveNow.Views.ViewModels;

using System.Windows;

namespace ArchiveNow.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly IArchiveNowProfileRepository _repository;

        public SettingsWindow(IConfigurationProvider<ArchiveNowConfiguration> configurationProvider, IArchiveNowProfileRepository repository)
        {
            InitializeComponent();

            _repository = repository;

            DataContext = new SettingsViewModel(configurationProvider);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var profiles = _repository.LoadAll();

            this.ProfilesListView.ItemsSource = profiles;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm)
            {
                vm.Save();
            }

            Close();
        }
    }
}
