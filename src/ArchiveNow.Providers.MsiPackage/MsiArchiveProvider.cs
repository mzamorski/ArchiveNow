using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

using ArchiveNow.Core;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.PasswordProviders;

using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

using WixFile = WixSharp.File;

namespace ArchiveNow.Providers.MsiPackage
{
    public class MsiArchiveProvider : ArchiveProviderBase
    {
        private readonly IArchiveEntryTransform _entryTransform;
        private readonly Project _project;
        private readonly InstallDir _installDir;

        public override string FileExtension => "msi";

        public MsiArchiveProvider(IArchiveFilePathBuilder pathBuilder, IArchiveEntryTransform entryTransform, IPasswordProvider passwordProvider)
            : base(pathBuilder, passwordProvider)
        {
            _entryTransform = entryTransform;

            string archiveFileName = Path.GetFileNameWithoutExtension(ArchiveFilePath);

            Compiler.EmitRelativePaths = true;
            Compiler.PreserveTempFiles = true;
            Compiler.ToolsOutputReceived += CompilerOnToolsOutputReceived;

            _project = new Project("ArchiveNow")
            {
                GUID = Guid.NewGuid(),
                UI = WUI.WixUI_InstallDir,
                Language = CultureInfo.CurrentCulture.Name,
                InstallPrivileges = InstallPrivileges.limited,
                ControlPanelInfo = new ProductInfo {Manufacturer = "ArchiveNow"},
                OutFileName = archiveFileName
            };

            _project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

            _installDir = new InstallDir(_entryTransform.RootPath);
            _project.AddDir(_installDir);

            //_project.Load += ProjectOnLoad;
            //_project.BeforeInstall += args =>
            //{
            //    args.InstallDir = Directory.GetCurrentDirectory();
            //};
        }

        private void ProjectOnLoad(SetupEventArgs e)
        {
            //e.Session["InstallDir"] = @"F:\";
        }

        private void CompilerOnToolsOutputReceived(string data)
        {
            Debug.WriteLine(data);
        }

        public override void AddDirectory(string path)
        {
            var relPath = _entryTransform.Transform(path);

            //_project.AddDir(new Dir(relPath));
        }

        public override void Add(string path)
        {
            //var relPath = _entryTransform.Transform(path);

            var directoryPath = Path.GetDirectoryName(path);
            Dir dir;

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                dir = _installDir;
            }
            else
            {
                dir = _project.FindDir(directoryPath) ?? new Dir(directoryPath);
            }

            dir.AddFile(new WixFile(path));

            //if (directoryPath != null)
            //{
            //    var directories = directoryPath.Split(Path.DirectorySeparatorChar);
            //    foreach (var directory in directories)
            //    {
                    
            //    }
            //}

            //_project.AddDir(new Dir("[INSTALLDIR]", new Files(path)));
            _project.AddDir(dir);
        }

        public override void BeginUpdate()
        { }

        public override void CommitUpdate()
        {
            var path = Compiler.BuildMsi(_project);
            if (path == null)
            {
                throw new ArchiveNowException($"{nameof(MsiArchiveProvider)}: Unable to create output file!");
            }
        }

        public override void AbortUpdate()
        { }
    }
}