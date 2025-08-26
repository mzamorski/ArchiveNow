using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Configuration.Readers;
using ArchiveNow.Core.Loggers;
using ArchiveNow.Integration;
using ArchiveNow.Service;
using ArchiveNow.Utils;
using ArchiveNow.Utils.IO;
using ArchiveNow.Utils.Windows;
using ArchiveNow.Views;

using Fclp;

namespace ArchiveNow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string ConfigFileName = "ArchiveNow.conf";
        private const string ProfilesDirectoryName = "Profiles";
        private const string ClientName = "ArchiveNow";

        private static string UserDataDirectoryPath
           => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ClientName);

        private readonly string _workingDirectory;
        private readonly string _configFilePath;
        private readonly string _profilesDirectoryPath;

        private readonly IArchiveNowProfileRepository _profileRepository;
        private readonly IArchiveNowLogger _logger = new FileLogger();

        public App()
        {
            _workingDirectory = AssemblyHelper.GetExecutingDirectory();
            _configFilePath = Path.Combine(_workingDirectory, ConfigFileName);
            _profilesDirectoryPath = Path.Combine(UserDataDirectoryPath, ProfilesDirectoryName);

            _profileRepository = new ArchiveNowProfileRepository(_profilesDirectoryPath, _logger);

            //Mapper.Initialize(
            //    config =>
            //    {
            //        config.CreateMap<ArchiveNowProfile, ArchiveNowProfileViewModel>()
            //            .ForMember(
            //                dest => dest.IgnoreEntries,
            //                expression =>
            //                    expression.MapFrom(
            //                        src =>
            //                            src.IgnoredFiles.Select(
            //                                pattern =>
            //                                    new ArchiveNowProfileIgnoreEntryViewModel(pattern, IgnoreEntryType.File))
            //                                .Concat(
            //                                    src.IgnoredDirectories.Select(
            //                                        pattern =>
            //                                            new ArchiveNowProfileIgnoreEntryViewModel(pattern, IgnoreEntryType.Directory))
            //                                )
            //                    )
            //            );

            //        config.CreateMap<ArchiveNowProfileViewModel, ArchiveNowProfile>()
            //            .ForMember(
            //                dest => dest.IgnoredFiles,
            //                expression =>
            //                    expression
            //                    .MapFrom(
            //                        src => src.IgnoreEntries
            //                            .Where(entry => entry.EntryType == IgnoreEntryType.File)
            //                            .Select(entry => entry.Pattern)))
            //            .ForMember(
            //                dest => dest.IgnoredDirectories,
            //                expression =>
            //                    expression.MapFrom(
            //                        src =>
            //                            src.IgnoreEntries
            //                                .Where(entry => entry.EntryType == IgnoreEntryType.Directory)
            //                                .Select(entry => entry.Pattern)))
            //            ;
            //    });
            

            //var viewModel = new ArchiveNowProfileViewModel();
            //viewModel.Name = "Foo";
            //viewModel.IgnoreEntries = new ObservableCollection<ArchiveNowProfileIgnoreEntryViewModel>();
            //viewModel.IgnoreEntries.Add(new ArchiveNowProfileIgnoreEntryViewModel("A", IgnoreEntryType.File));
            //viewModel.IgnoreEntries.Add(new ArchiveNowProfileIgnoreEntryViewModel("B", IgnoreEntryType.File));
            //viewModel.IgnoreEntries.Add(new ArchiveNowProfileIgnoreEntryViewModel("C", IgnoreEntryType.Directory));

            //var model = Mapper.Map<ArchiveNowProfile>(viewModel);

            //model.AfterFinishedActions.Add(new MoveToDirectoryAction("dupa"));

            ////var model = _repository.Find("Test");
            ////var viewModel = Mapper.Map<ArchiveNowProfileViewModel>(model);

            //_repository.Save(model);
        }

        private void OnAppStartup(object sender, StartupEventArgs e)
        {
            _logger.Debug($"Initialization...\n" 
                + "--\n"
                + $"WorkingDirectory: {_workingDirectory}\n"
                + $"ConfigurationFilePath: {_configFilePath}\n"
                + $"ProfilesDirectoryPath: {_profilesDirectoryPath}\n"
                + "--"
            );

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var arguments = ParseArguments(e.Args);
            if (arguments.IsEmpty)
            {
                Current.Shutdown();
                return;
            }

            try
            {
                Open(arguments);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Current.Shutdown();
        }

        private void Open(ArchiveNowArguments arguments)
        {
            if (arguments.Mode == ArchiveNowMode.ShowSettings)
            {
                OpenSettingsWindow();
            }
            else if (arguments.Mode == ArchiveNowMode.CreateProfile)
            {
                OpenProfileEditorWindow();
            }
            else if (arguments.Mode == ArchiveNowMode.Archiving)
            {
                if (arguments.UseProfile)
                {
                    OpenArchiveWindow(arguments.Paths, arguments.ProfileFilePath);
                }
                else
                {
                    OpenArchiveWindow(arguments.Paths);
                }
            }
            else if (arguments.Mode == ArchiveNowMode.Integrate)
            {
                Integrate();
            }
            else
            {
                //OpenInfoWindow();
                OpenSettingsWindow();
            }
        }

        private void Integrate()
        {
            IArchiveNowShellIntegrator integrator = new DefaultArchiveNowShellIntegrator();
            integrator.Integrate();
        }

        private ArchiveNowArguments ParseArguments(string[] args)
        {
            _logger.Debug($"Arguments: {args.Concat(" ")}");

            var arguments = new ArchiveNowArguments();

            var parser = new FluentCommandLineParser { OptionFormatter = new CommandLineOptionFormatter() };
            //parser.ErrorFormatter = new CommandLineParserErrorFormatter();

            parser.Setup<List<string>>("paths")
                .Callback(items =>
                    {
                        arguments.Paths = items;
                        arguments.Mode = ArchiveNowMode.Archiving;
                    }
                )
                .SetDefault(new List<string>(0));

            parser.Setup<string>("profile-name")
                .Callback(arg => {
                    arguments.ProfileName = arg;
                })
                .SetDefault(string.Empty);

            parser.Setup<string>("profile-file")
                .Callback(arg => {
                    arguments.ProfileFilePath = arg;
                })
                .SetDefault(string.Empty);

            parser.Setup<bool>("create-profile")
                .Callback(arg => arguments.Mode = ArchiveNowMode.CreateProfile);

            parser.Setup<bool>("edit-profile")
                .Callback(arg => arguments.Mode = ArchiveNowMode.EditProfile);

            parser.Setup<bool>("show-settings")
                .Callback(arg => arguments.Mode = ArchiveNowMode.ShowSettings);

            parser.Setup<bool>("integrate")
                .Callback(arg => arguments.Mode = ArchiveNowMode.Integrate);

            parser.SetupHelp("?", "help")
                .Callback(helpText =>
                    {
                        string version = string.Empty;
                        try
                        {
                            if (File.Exists(_configFilePath))
                            {
                                var configurationProvider = new ArchiveNowConfigurationProvider(_configFilePath, _profileRepository);
                                version = configurationProvider.Read().Version;
                            }
                        }
                        catch
                        {
                            // ignore configuration errors when showing help
                        }

                        var versionInfo = string.IsNullOrWhiteSpace(version) ? string.Empty : $"Version: {version}\n\n";
                        var message = $"{versionInfo}Invalid or missing argument!\n\nExpected options:\n{helpText}";

                        MessageBox.Show(message, ClientName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                );

            var result = parser.Parse(args);

            if (result.HasErrors || result.AdditionalOptionsFound.Any())
            {
                parser.HelpOption.ShowHelp(parser.Options);
                _logger.Error("Invalid argument!");

                return ArchiveNowArguments.Empty;
            }

            return arguments;
        }

        private void OpenSettingsWindow()
        {
            var window = new SettingsWindow(_profileRepository);
            var success = WindowHelper.Launch(window);
        }

        private void OpenProfileEditorWindow()
        {
            Process.Start(_profilesDirectoryPath);
            return;

            /*
            var viewModel = new ArchiveNowProfileViewModel();

            var filePatterns =
                profile.IgnoredFiles.Select(
                    i => new IgnoreEntryViewModel { Pattern = i, EntryType = IgnoreEntryType.File });

            var dictionaryPatterns =
                profile.IgnoredFiles.Select(
                    i => new IgnoreEntryViewModel { Pattern = i, EntryType = IgnoreEntryType.Directory });

            viewModel.IgnoreEntries =
                new ObservableCollection<IgnoreEntryViewModel>(filePatterns.Concat(dictionaryPatterns));
                */

            //var profile = new ArchiveNowProfile();
            //var profileView = Mapper.Map<ArchiveNowProfileViewModel>(profile);


            /*

            var profileView = new ArchiveNowProfileViewModel();

            var window = new ProfileEditorWindow
            {
                DataContext = profileView
            };

            var success = WindowHelper.Launch(window);
            if (!success)
            {
                return;
            }

            var profile = Mapper.Map<ArchiveNowProfile>(profileView);

            _profileRepository.Save(profile);

            */
        }

        private void OpenArchiveWindow(IEnumerable<string> paths, string profileFilePath = null)
        {
            if (!File.Exists(_configFilePath))
            {
                MessageBox.Show(
                    "Configuration file doesn't exist!\n\n" + _configFilePath,
                    $"{ClientName}",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            ArchiveNowConfiguration configuration;
            try
            {
                var configurationProvider = new ArchiveNowConfigurationProvider(_configFilePath, _profileRepository);
                configuration = configurationProvider.Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Invalid configuration file:\n\n{_configFilePath}\n\n{ex.Message}",
                    $"{ClientName}",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            IArchiveNowProfile currentProfile = NullArchiveNowProfile.Instance;

            try
            {
                currentProfile = OpenProfile(profileFilePath, configuration);
            }
            catch (IOException)
            {
                MessageBox.Show("Profile was not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            var service = new ArchiveNowService(configuration, currentProfile, _logger);
            //var service = new FakeArchiveNowService(configuration, currentProfile, _logger);

            foreach (var path in paths)
            {
                _logger.Info($"Archiving path: '{path}'");

                if (!path.PathExists())
                {
                    _logger.Warning($"Path '{path}' doesn't exist!");

                    var result =
                        MessageBox.Show("Path doesn't exist and will be skipped!\n\nDo you want to continue?", "Error",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        break;
                    }

                    continue;
                }

                var progress = new ArchiveProgressWindow(service, path);
                progress.ShowDialog();
            }
        }

        private IArchiveNowProfile OpenProfile(string profileFilePath, IArchiveNowConfiguration configuration = null)
        {
            IArchiveNowProfile profile;

            bool useUserProfile = (profileFilePath != null);
            if (useUserProfile)
            {
                profile = _profileRepository.Open(profileFilePath);
            }
            else
            {
                profile = configuration?.DefaultProfile;
            }

            return profile ?? NullArchiveNowProfile.Instance;
        }
    }
}
