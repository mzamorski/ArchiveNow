namespace ArchiveNow.Configuration.Readers
{
    public class SystemRegistryConfigurationProvider<T> : IConfigurationProvider<T> 
        where T : class
    {
        public T Read()
        {
            throw new System.NotImplementedException();
        }

        public void Write(T configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}