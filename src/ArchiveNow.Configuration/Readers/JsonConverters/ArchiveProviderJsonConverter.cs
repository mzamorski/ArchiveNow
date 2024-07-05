using System;

using ArchiveNow.Providers.Core;

using Newtonsoft.Json;

namespace ArchiveNow.Configuration.Readers.JsonConverters
{
    public class ArchiveProviderJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IArchiveProvider provider = null;

            return provider;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IArchiveProvider).IsAssignableFrom(objectType);
        }
    }
}