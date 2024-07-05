using System;
using ArchiveNow.Utils.IO;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Tests.ArchiveNow.Utils
{
    [TestFixture]
    public class PathExtensionsTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase("C:\\foo\\bar", "bar")]
        [TestCase("C:\\foo\\bar.txt", "bar")]
        public void Build_RandomFileName(string path, string expected)
        {
            //Assert.AreEqual(expected, path.GetName());
        }
    }
}
