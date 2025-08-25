namespace ArchiveNow.RemoteUploadHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "ArchiveNow.conf");
            //var configPath = @"E:\__PROJECTS\MY\ArchiveNow\ArchiveNow\bin\Debug\ArchiveNow.conf";
            var configuration = new RemoteUploadConfig() { Port = 5000, UploadsDirectory = @"E:\__PROJECTS\MY\ArchiveNow\uploads" }; 

            using (var host = new RemoteUploadHostListener(configuration))
            {
                host.Start();
                Console.WriteLine("Remote upload host running. Press Enter to stop...");
                Console.ReadLine();
                host.Stop();
            }
        }
    }
}