using System.Windows;

using ArchiveNow.Configuration.Profiles;

namespace ArchiveNow.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly IArchiveNowProfileRepository _repository;

        public SettingsWindow(IArchiveNowProfileRepository repository)
        {
            _repository = repository;

            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var profiles = _repository.LoadAll();
            foreach (var x in profiles)
            {
                
            }

            this.ProfilesListView.ItemsSource = profiles;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var profile = new ArchiveNowProfile();
            profile.Name = "Visual Studio Project";
            profile.IgnoredDirectories.Add(".svn");
            //profile.FileNameBuilder = FileNameBuilderFactory.Build()


            _repository.Save(profile);
        }

    }
}
