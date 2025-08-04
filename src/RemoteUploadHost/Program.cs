using System;
using System.IO;
using System.Threading.Tasks;

namespace RemoteUploadHost
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "ArchiveNow.conf");
            var configuration = RemoteUploadConfig.Load(configPath);

            var service = new RemoteUploadService(configuration);
            await service.StartAsync();

            await Task.Delay(-1);
        }
    }
}
