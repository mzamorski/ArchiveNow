using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchiveNow.Configuration.Readers;
using ArchiveNow.Core.Loggers;
using ArchiveNow.Utils;
using EnsureThat;
using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Profiles
{
    public class ArchiveNowProfileRepository : IArchiveNowProfileRepository
    {
        private const string DefaultProfileFileNameExtension = "profile";

        public IArchiveNowLogger Logger { get; }

        public ArchiveNowProfileProvider Provider { get; } = new ArchiveNowProfileProvider();

        public string DirectoryPath { get; }

        public ArchiveNowProfileRepository(string directoryPath, IArchiveNowLogger logger)
        {
            DirectoryPath = directoryPath;
            Logger = logger ?? EmptyArchiveNowLogger.Instance;
        }

        /// <summary>
        /// TODO: Zrefaktoryzowac z LoadAll() (obsługa exceptiona)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IArchiveNowProfile Open(string filePath)
        {
            IArchiveNowProfile profile;

            try
            {
                string path = Path.IsPathRooted(filePath) 
                    ? filePath 
                    : Path.Combine(DirectoryPath, filePath);

                profile = Provider.Read(path);
            }
            catch (Exception exception)
            {
                profile = new IncorrectArchiveNowProfile(exception.Message, Path.GetFileName(filePath));
            }

            return profile;
        }

        public IEnumerable<IArchiveNowProfile> LoadAll()
        {
            var files = Directory.EnumerateFiles(DirectoryPath, "*.profile");
            foreach (var filePath in files)
            {
                IArchiveNowProfile profile;
                try
                {
                    profile = Provider.Read(filePath);
                }
                catch (Exception exception)
                {
                    profile = new IncorrectArchiveNowProfile(exception.Message, Path.GetFileName(filePath));
                }

                yield return profile;
            }
        }

        public void Save(IArchiveNowProfile profile)
        {
            EnsureArg.IsNotNull(profile);

            if (!profile.IsValid(out var message))
            {
                throw new InvalidOperationException($"This profile cannot be saved because it is invalid!\n\n{message}");
            }

            var profileFileName = Path.ChangeExtension(profile.Name, DefaultProfileFileNameExtension);
            var profileFilePath = Path.Combine(DirectoryPath, profileFileName);

            profile.CreateDate = SystemTime.Now().Date;
            profile.ModifyDate = SystemTime.Now().Date;

            var writer = new ArchiveNowProfileProvider(profileFilePath);
            writer.Write((ArchiveNowProfile)profile);
        }

        public IArchiveNowProfile Find(string name, IArchiveNowProfile defaultProfile = null)
        {
            EnsureArg.IsNotNull(name);

            defaultProfile = defaultProfile ?? NullArchiveNowProfile.Instance;

            return LoadAll()
                .Where(profile => profile.Name.Equals(name))
                .DefaultIfEmpty(defaultProfile)
                .First();
        }
    }
}
