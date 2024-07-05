using System.IO;
using System.Reflection;

namespace ArchiveNow.Utils
{
    public static class AssemblyHelper
    {
        public static string GetExecutingDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
