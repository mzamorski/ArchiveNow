using ArchiveNow.Utils.IO;
using NUnit.Framework;

namespace Tests.ArchiveNow.Utils
{
    [TestFixture]
    public class PathExtensionsTests
    {
        [SetUp]
        public void Setup()
        { }

        [TestCase("C:\\foo\\bar", "bar", "")]
        [TestCase("C:\\foo\\bar.txt", "bar", "txt")]
        public void Build_RandomFileName(string path, string name, string extension)
        {
            Assert.That(name, Is.EqualTo(path.GetName(out var ext)));
            Assert.That(ext, Is.EqualTo(extension));
        }
    }
}
