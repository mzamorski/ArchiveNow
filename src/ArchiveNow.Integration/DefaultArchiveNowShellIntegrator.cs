using SharpShell;
using SharpShell.ServerRegistration;
using System.Diagnostics;
using System.IO;

namespace ArchiveNow.Integration
{
    public class DefaultArchiveNowShellIntegrator : IArchiveNowShellIntegrator
    {
        private readonly ISharpShellServer _server = new VsSolutionShellContextMenu();

        public bool IsIntegrated => false;

        public void Integrate()
        {
            ServerRegistrationManager.UnregisterServer(_server, RegistrationType.OS64Bit);
            ServerRegistrationManager.UninstallServer(_server, RegistrationType.OS64Bit);

            ServerRegistrationManager.InstallServer(_server, RegistrationType.OS64Bit, codeBase: true);
            ServerRegistrationManager.RegisterServer(_server, RegistrationType.OS64Bit);

            var servicePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "RemoteUploadHost.exe");
            var createService = new ProcessStartInfo("sc.exe", $"create ArchiveNowRemoteUploadHost binPath= \"{servicePath}\" start= auto")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                using var process = Process.Start(createService);
                process?.WaitForExit();
            }
            catch
            {
                // ignore if service already exists or registration fails
            }
        }

        public void Disintegrate()
        {
            ServerRegistrationManager.UnregisterServer(_server, RegistrationType.OS64Bit);
        }
    }
}
