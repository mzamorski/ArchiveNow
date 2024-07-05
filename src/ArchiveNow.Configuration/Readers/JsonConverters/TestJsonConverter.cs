using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ArchiveNow.Configuration.Readers.JsonConverters
{
    public class TestJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dic = serializer.Deserialize(reader, objectType);

            return new SortedDictionary<string, string>();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
