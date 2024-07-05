using System;
using ArchiveNow.Providers.Core.FileNameBuilders;
using NUnit.Framework;

namespace Tests.ArchiveNow.FileNameBuilder
{
    [TestFixture]
    public class FileNameBuilderTests
    {
        [Test]
        public void Build_RandomFileName()
        {
            var builder = new RandomFileNameBuilder();
            var fileName = builder.Build();

            Assert.IsFalse(string.IsNullOrWhiteSpace(fileName));
        }

        [Test]
        public void Build_AddDateTimeToFileName()
        {
            string directoryName = "Test";
            var dateTime = new DateTime(2016, 1, 2, 12, 30, 45);

            var builder = new AddDateTimeFileNameBuilder();
            IFileNameBuilderContext ctx = new FileNameBuilderContext(directoryName, dateTime);

            var formattedFileName = builder.Build(ctx);

            Assert.AreEqual("Test_20160102-1230", formattedFileName);
        }

        [Test]
        public void Build_ParentDirectoryFileName()
        {
            string directoryPath = "C:\\MyProject\\trunk";
            var currentDateTime = new DateTime(2016, 1, 2, 12, 30, 45);

            var builder = new ParentDirectoryNameFileNameBuilder();
            IFileNameBuilderContext ctx = new FileNameBuilderContext(directoryPath, currentDateTime);

            var formattedFileName = builder.Build(ctx);

            Assert.AreEqual("MyProject", formattedFileName);
        }

        [Test]
        public void Build_ParentDirectoryWithDateFileName()
        {
            string directoryPath = "C:\\MyProject\\trunk";
            var currentDateTime = new DateTime(2016, 1, 2, 12, 30, 45);

            var builder = new CompositeFileNameBuilder(
                new ParentDirectoryNameFileNameBuilder(),
                new AddDateTimeFileNameBuilder());

            IFileNameBuilderContext ctx = new FileNameBuilderContext(directoryPath, currentDateTime);

            var formattedFileName = builder.Build(ctx);

            Assert.AreEqual("MyProject_20160102-1230", formattedFileName);
        }

        /*
        [Test]
        public void Foo()
        {
            string directoryPath = "C:\\MyProject\\trunk";

            var store = new PreferencesStore("AddVersion");
            var builder = new AddVersionFileNameBuilder(store);

            IFileNameBuilderContext ctx = new FileNameBuilderContext(directoryPath);

            var formattedFileName = builder.Build(ctx);

            Assert.AreEqual("trunk-1", formattedFileName);
        }
        */
    }
}
