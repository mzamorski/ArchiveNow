namespace ArchiveNow.RemoteUpload.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var configPath = Path.Combine(AppContext.BaseDirectory, "ArchiveNow.conf");
            var configuration = new RemoteUploadConfiguration() { Port = 5000, UploadsDirectory = @"uploads" }; 

            using (var host = new RemoteUploadServer(configuration))
            {
                host.Start();

                Console.WriteLine("Remote upload host running. Press Enter to stop...");
                Console.ReadLine();

                host.Stop();
            }
        }
    }
}