namespace ArchiveNow.Configuration
{
    public class RemoteUploadHostConfiguration
    {
        public int Port { get; set; }

        public string UploadsDirectory { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Port: {Port}, UploadsDirectory: {UploadsDirectory}";
        }
    }
}
