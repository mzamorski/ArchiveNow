using System;
using System.IO;

using ArchiveNow.Configuration.Profiles;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Readers.JsonConverters
{
    public class ProfileJsonConverter : JsonConverter
    {
        private readonly IArchiveNowProfileRepository _profileRepository;

        public ProfileJsonConverter(IArchiveNowProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var profile = value as IArchiveNowProfile;
            if (profile == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(profile.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var item = JToken.Load(reader);

            var value = (item.Type != JTokenType.String)
                ? string.Empty
                : item.Value<string>();

            // Open by profile file path.
            if (Path.HasExtension(value))
            {
                return _profileRepository.Open(value);
            }

            // Find by profile name;
            return _profileRepository.Find(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IArchiveNowProfile).IsAssignableFrom(objectType);
        }
    }
}