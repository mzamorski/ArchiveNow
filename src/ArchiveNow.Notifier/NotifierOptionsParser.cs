using System;
using Fclp;


namespace ArchiveNow.Notifier
{
    public static class NotifierOptionsParser
    {
        public static NotifierOptions Parse(string[] args)
        {
            var p = new FluentCommandLineParser<NotifierOptions>();

            p.Setup(o => o.Path)
               .As("path")
               .WithDescription("Path to the file");

            p.Setup(o => o.Title)
                .As("title")
                .WithDescription("Title of the toast notification");

            p.Setup(o => o.Message)
                .As("message")
                .Required()
                .WithDescription("Main text of the toast notification");

            //p.Setup(o => o.Client)
            //    .As("client")
            //    .WithDescription("Client name or host");

            //p.Setup(o => o.FileName)
            //    .As("file")
            //    .WithDescription("File name");

            //p.Setup<long>(o => o.Size)
            //    .As("size")
            //    .SetDefault(0)
            //    .WithDescription("File size in bytes");

            //p.Setup(o => o.Icon)
            //    .As("icon")
            //    .WithDescription("Path to an icon file for the toast");

            var result = p.Parse(args);
            if (result.HasErrors)
            {
                Console.Error.WriteLine("Argument parsing failed: " + result.ErrorText);
                Environment.Exit(1);
            }

            return p.Object;
        }
    }
}