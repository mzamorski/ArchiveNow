using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Configuration.Readers;
using ArchiveNow.Core.Loggers;
using ArchiveNow.Shell.Properties;
using ArchiveNow.Utils;

using NLog;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace ArchiveNow.Shell
{
    /// <summary>
    /// TODO: Przenieśc wszystkie argumenty cmd do osobnej klasy.
    /// </summary>
    [Guid("5ca6ff10-d301-4db8-8e3c-aef37b22ffca")]
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.DirectoryBackground)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class ArchiveNowShellContextMenu : SharpContextMenu
    {
        private const string ClientName = "ArchiveNow";
        private const string ClientFileName = "ArchiveNow.exe";
        private const string ProfilesDirectoryName = "Profiles";
        private const string ConfigFileName = "ArchiveNow.conf";

        private static string ClientDirectoryPath => AssemblyHelper.GetExecutingDirectory();

        private static string UserDataDirectoryPath
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ClientName);

        private static string ClientFilePath => Path.Combine(ClientDirectoryPath, ClientFileName);
        private static string ConfigFilePath => Path.Combine(ClientDirectoryPath, ConfigFileName);
        private static string ProfilesDirectoryPath => Path.Combine(UserDataDirectoryPath, ProfilesDirectoryName);

        private readonly ContextMenuStrip _contextMenu;
        private readonly ToolStripMenuItem _profilesMenuItem;
        private readonly ToolStripMenuItem _archiveNowMenuItem;

        private readonly IArchiveNowConfiguration _configuration;
        private readonly IArchiveNowLogger _logger = new FileLogger();

        /// <summary>
        /// AssociationType.DirectoryBackground
        /// </summary>
        private bool IsDirectoryBackgroundSelected => Directory.Exists(FolderPath);

        /// <summary>
        /// AssociationType.Directory
        /// </summary>
        private bool IsDirectoryItemSelected => SelectedItemPaths.Any();

        public IEnumerable<string> SelectedPaths => IsDirectoryBackgroundSelected 
            ? new[] { FolderPath } 
            : SelectedItemPaths;

        public ArchiveNowShellContextMenu()
        {
            try
            {
                var configurationProvider = new ArchiveNowConfigurationProvider(ConfigFilePath, new ArchiveNowProfileRepository(ProfilesDirectoryPath, _logger));
                _configuration = configurationProvider.Read();

                //_logger.Debug($"Configuration: {_configuration}");
                    
                (_contextMenu, _archiveNowMenuItem, _profilesMenuItem) = CreateMenu(_configuration);

                CreateProfilesMenuItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"{ClientName} shell error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private (ContextMenuStrip contextMenu, ToolStripMenuItem archiveNowMenuItem, ToolStripMenuItem archiveNowWithProfileMenuItem) CreateMenu(IArchiveNowConfiguration configuration)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripSeparator());

            var archiveNowMenuItem = new ToolStripMenuItem
            {
                Text = ClientName, 
                Image = Resources.an_main_16px, ImageScaling = ToolStripItemImageScaling.None,
                Tag = NullArchiveNowProfile.Instance
            };
            archiveNowMenuItem.Click += OnArchiveNowMenuItemClick;
            contextMenu.Items.Add(archiveNowMenuItem);

            var archiveNowWithProfileMenuItem = new ToolStripMenuItem
            {
                Text = string.Format(Resources.ArchiveNowShellContextMenu_CreateMenu_ArchiveNowUsingProfile, ClientName),
                Image = Resources.an_using_profile_16px,
                ImageScaling = ToolStripItemImageScaling.None,
            };
            contextMenu.Items.Add(archiveNowWithProfileMenuItem);
            
            var createProfileMenu = new ToolStripMenuItem { Text = Resources.ArchiveNowShellContextMenu_CreateMenu_CreateProfile };
            createProfileMenu.Click += OnCreateProfileMenuClick;
            contextMenu.Items.Add(createProfileMenu);

            var settingsMenu = new ToolStripMenuItem { Text = Resources.ArchiveNowShellContextMenu_CreateMenu_Settings };
            settingsMenu.Click += OnSettingsMenuClick;
            contextMenu.Items.Add(settingsMenu);

            contextMenu.Items.Add(new ToolStripSeparator());

            return (contextMenu, archiveNowMenuItem, archiveNowWithProfileMenuItem);
        }

        private void CreateProfilesMenuItems()
        {
            if (IsProfilesDirectoryNotExists())
            {
                var errorProfileMenuItem = new ToolStripMenuItem
                {
                    Text = Resources.ArchiveNowShellContextMenu_CreateProfilesMenuItems_MissingProfilesFolder,
                    Enabled = false
                };
                errorProfileMenuItem.Click += OnErrorProfileMenuItemClick;

                _profilesMenuItem.DropDownItems.Add(errorProfileMenuItem);

                return;
            }

            var repository = new ArchiveNowProfileRepository(ProfilesDirectoryPath, _logger);
            var profiles = repository.LoadAll().ToList();

            if (profiles.None())
            {
                var emptyProfileMenuItem = new ToolStripMenuItem
                {
                    Text = Resources.ArchiveNowShellContextMenu_CreateProfilesMenuItems_CreateNewProfile,
                    Enabled = true
                };
                emptyProfileMenuItem.Click += OnEmptyProfileMenuItemClick;

                _profilesMenuItem.DropDownItems.Add(emptyProfileMenuItem);

                return;
            }

            foreach (var profile in profiles)
            {
                var profileMenuItem = new ToolStripMenuItem { Tag = profile };

                if (profile.IsValid(out var errorMessage))
                {
                    profileMenuItem.Text = profile.Name;

                    profileMenuItem.Click += OnArchiveNowMenuItemClick;
                    profileMenuItem.Enabled = true;
                }
                else
                {
                    profileMenuItem.Text = profile.Name ?? errorMessage;
                    profileMenuItem.Enabled = false;
                }

                _profilesMenuItem.DropDownItems.Add(profileMenuItem);
            }
        }

        private void OnCreateProfileMenuClick(object sender, EventArgs e)
        {
            OpenCreateProfile();
        }

        private static bool IsProfilesDirectoryNotExists()
        {
            return !Directory.Exists(ProfilesDirectoryPath);
        }

        private static bool IsClientFileNotExists()
        {
            return !File.Exists(ClientFilePath);
        }

        private void OnSettingsMenuClick(object sender, EventArgs eventArgs)
        {
            OpenSettingsArchiveNow();
        }

        private void OpenArchiveNow(string args)
        {
            try
            {
                if (IsClientFileNotExists())
                {
                    MessageBox.Show($"{ClientName} client not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Process.Start(ClientFilePath, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnArchiveNowMenuItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var profile = (IArchiveNowProfile)item.Tag;

            var profileArg = profile.IsEmpty 
                ? string.Empty 
                : $"--profile-file \"{profile.Location}\"";

            var builder = new StringBuilder();
            foreach (var directoryPath in SelectedPaths)
            {
                builder.AppendFormat("\"{0}\" ", directoryPath);
            }

            OpenArchiveNow($"--paths {builder} {profileArg}");
        }

        protected override bool CanShowMenu()
        {
            return IsDirectoryBackgroundSelected || IsDirectoryItemSelected;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            //MessageBox.Show(_configuration.ToString());

            var selectedPaths = new List<string>(base.SelectedItemPaths);
            if (selectedPaths.None())
            {
                selectedPaths.Add(FolderPath);
            }

            foreach (var selectedPath in selectedPaths)
            {
                if (_configuration.DirectoryProfileMap.TryGetValue(selectedPath, out IArchiveNowProfile pathProfile))
                {
                    _archiveNowMenuItem.Text += $" (profile: `{pathProfile.Name}`)";
                }
            }

            return _contextMenu;
        }

        protected override void LogError(string message, Exception exception = null)
        {
            base.LogError(message, exception);

            MessageBox.Show(message, $"{ClientName} shell error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnErrorProfileMenuItemClick(object sender, EventArgs e)
        {
            OpenSettingsArchiveNow();
        }

        private void OnEmptyProfileMenuItemClick(object sender, EventArgs e)
        {
            //OpenCreateProfile();
            Process.Start(ProfilesDirectoryPath);
        }

        private void OpenSettingsArchiveNow()
        {
            OpenArchiveNow("--show-settings");
        }

        private void OpenCreateProfile()
        {
            OpenArchiveNow("--create-profile");
        }
    }
}
