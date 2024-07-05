using System;

using ArchiveNow.Providers.Core.FileNameBuilders;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Readers.JsonConverters
{
    public class FileNameBuilderJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var item = JToken.Load(reader);

            var value = (item.Type != JTokenType.String) 
                ? string.Empty 
                : item.Value<string>();

            return FileNameBuilderFactory.Build(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IFileNameBuilder).IsAssignableFrom(objectType);
        }
    }
}