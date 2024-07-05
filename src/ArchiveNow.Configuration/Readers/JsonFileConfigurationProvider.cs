using System;
using System.Globalization;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchiveNow.Configuration.Readers
{
    public class JsonFileConfigurationProvider<T> : IConfigurationProvider<T> 
        where T : class, new()
    {
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly JsonSerializer _jsonSerializer;

        public static JsonFileConfigurationProvider<T> Instance { get; } = new Lazy<JsonFileConfigurationProvider<T>>().Value;

        protected JsonSerializerSettings SerializerSettings => _jsonSettings;

        protected string FilePath { get; }

        public JsonFileConfigurationProvider(string filePath, params JsonConverter[] converters)
            : this(converters)
        {
            FilePath = filePath;
        }

        public JsonFileConfigurationProvider(params JsonConverter[] converters)
        {
            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            _jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = dateTimeFormat.ShortDatePattern,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = converters
            };

            _jsonSerializer = JsonSerializer.Create(_jsonSettings);
            _jsonSerializer.Formatting = Formatting.Indented;
        }

        public virtual T Read()
        {
            return Read(FilePath);
        }

        public virtual T Read(string filePath)
        {
            var json = File.ReadAllText(filePath);

            var jsonObject = JObject.Parse(json);
            var configuration = jsonObject.ToObject<T>(_jsonSerializer);

            OnAfterDeserialize(configuration, jsonObject);

            return configuration;
        }

        public void Write(T configuration, string filePath)
        {
            //var json = JsonConvert.SerializeObject(configuration, Formatting.Indented, _jsonSettings);
            //File.WriteAllText(_filePath, json);

            using (var writer = new StreamWriter(filePath))
            {
                _jsonSerializer.Serialize(writer, configuration, typeof(T));
            }
        }

        public void Write(T configuration)
        {
            Write(configuration, FilePath);
        }

        protected virtual void OnAfterDeserialize(T configuration, JObject jsonObject)
        { }
    }
}