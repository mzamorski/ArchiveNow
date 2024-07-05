using System;

using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Configuration.Readers.JsonConverters;
using ArchiveNow.Configuration.Readers.JsonResolvers;

namespace ArchiveNow.Configuration.Readers
{
    public class ArchiveNowProfileProvider : JsonFileConfigurationProvider<ArchiveNowProfile>
    {
        private const string LocationPropertyName = "Location";

        public ArchiveNowProfileProvider()
            : this(string.Empty)
        { }

        public ArchiveNowProfileProvider(string filePath) 
            : base(filePath,
                new AfterFinishedActionJsonConverter(),
                new FileNameBuilderJsonConverter(), 
                new ArchiveProviderJsonConverter()
        )
        {
            var resolver = new PropertyFilterSerializerContractResolver { IgnoreReadonly = true };
            resolver.IgnoreProperty(typeof(ArchiveNowProfile), LocationPropertyName);

            SerializerSettings.ContractResolver = resolver;
        }

        public override ArchiveNowProfile Read(string path)
        {
            ArchiveNowProfile profile = base.Read(path);
            profile.Location = path;

            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    profile = IncorrectArchiveNowProfile.Instance;
            //}

            return profile;
        }

        //protected override void OnAfterDeserialize(ArchiveNowProfile profile, JObject jsonObject)
        //{
        //    base.OnAfterDeserialize(profile, jsonObject);

        //    JToken profileName;
        //    if (jsonObject.TryGetValue("ProviderName", StringComparison.Ordinal, out profileName))
        //    {
        //        profile.ArchiveProvider = ArchiveProviderFactory.Build(
        //            profileName.Value<string>(),
        //            new DefaultArchiveFilePathBuilder());
        //    }
        //}
    }
}
