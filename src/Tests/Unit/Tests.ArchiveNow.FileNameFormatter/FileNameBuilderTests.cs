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

            Assert.That(string.IsNullOrWhiteSpace(fileName), Is.False);
        }

        [Test]
        public void Build_AddDateTimeToFileName()
        {
            string directoryName = "Test";
            var dateTime = new DateTime(2016, 1, 2, 12, 30, 45);

            var builder = new AddDateTimeFileNameBuilder();
            IFileNameBuilderContext ctx = new FileNameBuilderContext(directoryName, dateTime);

            var formattedFileName = builder.Build(ctx);

            Assert.That("Test_20160102-1230", Is.EqualTo(formattedFileName));
        }

        [Test]
        public void Build_ParentDirectoryFileName()
        {
            string directoryPath = "C:\\MyProject\\trunk";
            var currentDateTime = new DateTime(2016, 1, 2, 12, 30, 45);

            var builder = new ParentDirectoryNameFileNameBuilder();
            IFileNameBuilderContext ctx = new FileNameBuilderContext(directoryPath, currentDateTime);

            var formattedFileName = builder.Build(ctx);

            Assert.That("MyProject-1230", Is.EqualTo(formattedFileName));
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

            Assert.That("MyProject_20160102-1230", Is.EqualTo(formattedFileName));
        }
    }
}
