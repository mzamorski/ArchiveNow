namespace ArchiveNow.Configuration.Readers
{
    public interface IConfigurationProvider<T>
        where T : class 
    {
        T Read();

        void Write(T configuration);
    }
}
