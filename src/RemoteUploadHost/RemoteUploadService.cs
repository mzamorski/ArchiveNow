using System.Threading.Tasks;

namespace RemoteUploadHost
{
    public class RemoteUploadService
    {
        private readonly RemoteUploadConfig _config;

        public RemoteUploadService(RemoteUploadConfig config)
        {
            _config = config;
        }

        public Task StartAsync()
        {
            // TODO: implement remote upload functionality
            return Task.CompletedTask;
        }
    }
}
