using System;
using System.Collections.Generic;

using ArchiveNow.Actions.Core;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Readers.JsonConverters
{
    public class AfterFinishedActionJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var actions = (List<IAfterFinishedAction>)existingValue;

            var items = JArray.Load(reader);

            foreach (var item in items)
            {
                var name = item["Name"].Value<string>();

                var parameters = new Dictionary<string, object>();

                var contextNode = item["Context"] as JObject;
                if (contextNode != null)
                {
                    foreach (var param in contextNode)
                    {
                        object value = param.Value.ToObject<string>();
                        if (value != null)
                        {
                            parameters.Add(param.Key, value);
                        }
                    }
                }

                IAfterFinishedAction action = AfterFinishedActionFactory.Build(name, parameters);

                if (item["BreakIfError"] is JValue breakIfErrorNode)
                {
                    action.BreakIfError = breakIfErrorNode.Value<bool>();
                }

                actions.Add(action);
            }

            return actions;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IList<IAfterFinishedAction>).IsAssignableFrom(objectType);
        }
    }
}