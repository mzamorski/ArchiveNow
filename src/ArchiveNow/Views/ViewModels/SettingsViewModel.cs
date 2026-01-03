using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Readers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArchiveNow.Views.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private ArchiveNowConfiguration _configuration;
        private IConfigurationProvider<ArchiveNowConfiguration> _configurationProvider;

        public ArchiveNowConfiguration Configuration => _configuration;

        public SettingsViewModel(IConfigurationProvider<ArchiveNowConfiguration> configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _configuration = _configurationProvider.Read();
        }

        public bool CloseWindowOnSuccess
        {
            get => _configuration.CloseWindowOnSuccess;
            set
            {
                if (_configuration.CloseWindowOnSuccess != value)
                {
                    _configuration.CloseWindowOnSuccess = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            _configurationProvider.Write(_configuration);
        }
    }
}
