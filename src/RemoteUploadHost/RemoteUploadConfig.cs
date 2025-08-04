using System;
using System.IO;
using System.Text.Json;

namespace RemoteUploadHost
{
    public class RemoteUploadConfig
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Directory { get; set; } = string.Empty;

        public static RemoteUploadConfig Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file '{path}' was not found.");

            var json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<RemoteUploadConfig>(json) ?? new RemoteUploadConfig();
            Validate(config);
            return config;
        }

        private static void Validate(RemoteUploadConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Host))
                throw new InvalidOperationException("Host is not configured.");

            if (config.Port <= 0 || config.Port > 65535)
                throw new InvalidOperationException("Port is invalid.");

            if (string.IsNullOrWhiteSpace(config.Directory))
                throw new InvalidOperationException("Directory is not configured.");

            if (!System.IO.Directory.Exists(config.Directory))
                throw new InvalidOperationException($"Directory '{config.Directory}' does not exist.");
        }
    }
}
