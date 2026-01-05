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
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented,
                
                Converters = converters,

                Error = (sender, args) =>
                {
                    Console.WriteLine($"Serialization error: {args.ErrorContext.Error.Message}");
                    Console.WriteLine($"Path: {args.ErrorContext.Path}");
                    args.ErrorContext.Handled = true; 
                }
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
            using (var streamWriter = new StreamWriter(filePath))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.Indentation = 4;
                jsonWriter.IndentChar = ' ';

                _jsonSerializer.Serialize(jsonWriter, configuration);
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