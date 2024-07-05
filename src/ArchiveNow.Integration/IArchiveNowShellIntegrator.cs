namespace ArchiveNow.Integration
{
    public interface IArchiveNowShellIntegrator
    {
        bool IsIntegrated { get; }

        void Integrate();
        void Disintegrate();
    }
}
