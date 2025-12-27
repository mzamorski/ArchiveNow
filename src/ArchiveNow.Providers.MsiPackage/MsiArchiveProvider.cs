using ArchiveNow.Core;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.PasswordProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using File = WixSharp.File;

namespace ArchiveNow.Providers.MsiPackage
{
    public class MsiArchiveProvider : ArchiveProviderBase
    {
        private const string AppName = "ArchiveNow";

        private readonly IArchiveEntryTransform _entryTransform;
        private readonly Project _project;
        private readonly InstallDir _installDir;
        private readonly Dictionary<string, Dir> _dirMap;

        public override string FileExtension => "msi";

        public MsiArchiveProvider(IArchiveFilePathBuilder pathBuilder, IArchiveEntryTransform entryTransform, IPasswordProvider passwordProvider)
            : base(pathBuilder, passwordProvider)
        {
            _entryTransform = entryTransform;

            SimulateLatency = true;

            var archiveFileName = Path.GetFileNameWithoutExtension(ArchiveFilePath);
            var outputDirectory = Path.GetDirectoryName(ArchiveFilePath) ?? throw new InvalidOperationException("ArchiveFilePath is null or invalid.");
            var sourceDirectoryPath = entryTransform.RootPath;

            Compiler.EmitRelativePaths = true;
            Compiler.PreserveTempFiles = true;
            Compiler.ToolsOutputReceived += CompilerOnToolsOutputReceived;

            _project = new Project(AppName)
            {
                GUID = Guid.NewGuid(),
                UI = WUI.WixUI_InstallDir,
                Language = CultureInfo.CurrentCulture.Name,
                InstallPrivileges = InstallPrivileges.limited,
                ControlPanelInfo = new ProductInfo { Manufacturer = AppName },
                OutFileName = archiveFileName,
                OutDir = outputDirectory,
                SourceBaseDir = sourceDirectoryPath
            };

            _project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);

            _installDir = new InstallDir(archiveFileName);
            _project.AddDir(_installDir);

            _dirMap = new Dictionary<string, Dir>(StringComparer.OrdinalIgnoreCase)
            {
                [string.Empty] = _installDir
            };
        }

        private void CompilerOnToolsOutputReceived(string data)
        {
            Debug.WriteLine(data);
        }

        public override void AddDirectory(string path)
        {
            var relPath = _entryTransform.Transform(path);
            string[] parts = relPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            string currentPath = string.Empty;
            Dir current = _installDir;

            foreach (var part in parts)
            {
                currentPath = Path.Combine(currentPath, part);

                if (!_dirMap.TryGetValue(currentPath, out Dir next))
                {
                    next = new Dir(part);

                    if (current.Dirs == null)
                    {
                        current.Dirs = new[] { next };
                    }
                    else
                    {
                        current.Dirs = current.Dirs.Concat(new[] { next }).ToArray();
                    }

                    _dirMap[currentPath] = next;
                }

                current = next;
            }

            ApplySimulateLatency(1);
        }

        public override void Add(string path)
        {
            var relPath = _entryTransform.Transform(path);
            var parentDir = Path.GetDirectoryName(relPath);

            if (!_dirMap.TryGetValue(parentDir, out Dir targetDir))
            {
                throw new InvalidOperationException($"Cannot add file '{path}' because directory '{parentDir}' was not added via AddDirectory().");
            }

            var list = new List<File>(targetDir.Files) { new File(path) };
            targetDir.Files = list.ToArray();

            ApplySimulateLatency(1);
        }

        public override void BeginUpdate(string sourcePath)
        {
            CurrentProgressMode = ProgressMode.Determinate;
        }

        public override void CommitUpdate(CancellationToken cancellationToken = default)
        {
            CurrentProgressMode = ProgressMode.Indeterminate;

            var path = _project.BuildMsi();
            if (path == null)
            {
                throw new ArchiveNowException($"{nameof(MsiArchiveProvider)}: Unable to create output file!");
            }
        }

        public override void AbortUpdate()
        { }
    }
}