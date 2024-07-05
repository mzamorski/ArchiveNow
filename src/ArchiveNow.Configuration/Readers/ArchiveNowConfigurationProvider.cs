using System;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Configuration.Readers.JsonConverters;
using ArchiveNow.Configuration.Readers.JsonResolvers;

using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Readers
{
    public class ArchiveNowConfigurationProvider : JsonFileConfigurationProvider<ArchiveNowConfiguration>
    {
        private readonly IArchiveNowProfileRepository _profileRepository;

        public ArchiveNowConfigurationProvider(string filePath, IArchiveNowProfileRepository profileRepository)
            : base(filePath, new ProfileJsonConverter(profileRepository))
        {
            _profileRepository = profileRepository;

            var resolver = new PropertyFilterSerializerContractResolver { IgnoreReadonly = true };
            SerializerSettings.ContractResolver = resolver;
        }

        protected override void OnAfterDeserialize(ArchiveNowConfiguration configuration, JObject jsonObject)
        {
            if (jsonObject.TryGetValue("DefaultProfileName", StringComparison.Ordinal, out var profileToken))
            {
                var profileName = profileToken.Value<string>();

                configuration.DefaultProfile = (profileName != null) 
                    ? _profileRepository.Find(profileName)
                    : NullArchiveNowProfile.Instance;
            }

            base.OnAfterDeserialize(configuration, jsonObject);
        }
    }
}