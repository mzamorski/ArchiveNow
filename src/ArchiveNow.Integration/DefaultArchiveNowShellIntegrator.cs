using SharpShell;
using SharpShell.ServerRegistration;

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
        }

        public void Disintegrate()
        {
            ServerRegistrationManager.UnregisterServer(_server, RegistrationType.OS64Bit);
        }
    }
}