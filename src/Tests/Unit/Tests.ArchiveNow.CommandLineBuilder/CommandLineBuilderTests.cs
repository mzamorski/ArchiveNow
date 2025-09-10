using ArchiveNow.Shell.CommandLineBuilder;
using NUnit.Framework;

namespace Tests.ArchiveNow.CommandLineBuilder
{
    public class CommandLineBuilderTests
    {
        [Test]
        public void Foo()
        {
            var builder = new ArchiveNowCommandLineBuilder();
            builder.AddPaths(new[] { @"home\foo", @"home\bar", @"home\baz" });

            var commandLine = builder.ToString();
            Assert.That("--paths \"home\\foo\" \"home\\bar\" \"home\\baz\"", Is.EqualTo(commandLine));
        }

        [Test]
        public void Bar()
        {
            var builder = new ArchiveNowCommandLineBuilder();
            builder.AddPaths(new[] { @"home\foo", @"home\bar" });
            builder.AddPaths(new[] { @"home\baz" });

            var commandLine = builder.ToString();
            Assert.That("--paths \"home\\foo\" \"home\\bar\" \"home\\baz\"", Is.EqualTo(commandLine));
        }

        [Test]
        public void Buz()
        {
            var builder = new ArchiveNowCommandLineBuilder();
            builder.AddPaths(new[] { @"home\foo", @"home\bar" });
            builder.AddPaths(new[] { @"home\baz" });
            builder.SetProfileName("Test.profile");

            var commandLine = builder.ToString();
            Assert.That("--paths \"home\\foo\" \"home\\bar\" \"home\\baz\"", Is.EqualTo(commandLine));
        }
    }
}
