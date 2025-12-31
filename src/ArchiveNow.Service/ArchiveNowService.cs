using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ArchiveNow.Actions.Core;
using ArchiveNow.Configuration;
using ArchiveNow.Configuration.Profiles;
using ArchiveNow.Configuration.Readers;
using ArchiveNow.Core.Loggers;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.FileNameBuilders;
using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Service.ArchiveProviders;
using ArchiveNow.Service.Helpers;
using ArchiveNow.Service.SearchFileProvider;
using ArchiveNow.Utils;
using ArchiveNow.Utils.IO;
using ArchiveNow.Utils.Threading;
using ArchiveNow.Utils.Windows;
using EnsureThat;

namespace ArchiveNow.Service
{
    public class ArchiveNowService : IArchiveNowService
    {
        private readonly IArchiveNowLogger _logger;

        public event EventHandler<string> FileAdded;
        public event EventHandler<string> FileAdding;

        public event EventHandler<string> DirectoryAdded;
        public event EventHandler<string> DirectoryAdding;
        public event EventHandler<int> Initialized;
        public event EventHandler<IArchiveResult> Finished;
        public event EventHandler<string> Commit;

        public event EventHandler<AfterFinishedActionEventArgs> ActionExecuting;
        public event EventHandler<AfterFinishedActionEventArgs> ActionExecuted;

        public IArchiveNowConfiguration Configuration { get; }

        public IArchiveNowProfile Profile { get; set; }

        public bool IsUsingProfile => !Profile.IsEmpty;

        public ArchiveNowService(IArchiveNowConfiguration configuration, IArchiveNowProfile profile, IArchiveNowLogger logger = null)
        {
            Configuration = configuration;
            Profile = profile;

            _logger = logger ?? EmptyArchiveNowLogger.Instance;
        }

        private void OnActionExecuted(IAfterFinishedAction action, IAfterFinishedActionResult result)
        {
            var resultMessage = result.IsSuccess ? "successfully." : $"unsuccessfully! Message: {result.Message}";
            _logger.Info($"Action {{{action.Name}}} was executed {resultMessage}");

            ActionExecuted?.Invoke(this, new AfterFinishedActionEventArgs(action, result));
        }

        private void OnActionExecuting(IAfterFinishedAction action)
        {
            ActionExecuting?.Invoke(this, new AfterFinishedActionEventArgs(action));
        }

        private void OnFinished(IArchiveResult result)
        {
            Finished?.Invoke(this, result);
        }

        private void OnInitialized(int numberOfFiles)
        {
            Initialized?.Invoke(this, numberOfFiles);
        }

        private void OnFileAdded(string path)
        {
            FileAdded?.Invoke(this, path);
        }

        private void OnDirectoryAdded(string path)
        {
            DirectoryAdded?.Invoke(this, path);
        }

        private void OnFileAdding(string path)
        {
            FileAdding?.Invoke(this, path);
        }

        private void OnDirectoryAdding(string path)
        {
            DirectoryAdding?.Invoke(this, path);
        }

        private void OnCommit(string archiveFilePath)
        {
            Commit?.Invoke(this, archiveFilePath);
        }

        public void Archive(string sourcePath, IArchiveNowProgress progressIndicator, CancellationToken cancelToken, PauseToken pauseToken)
        {
            // Expand the path - this is necessary if the path is relative (eg. contains dot "." or double-dots "..")
            sourcePath = Path.GetFullPath(sourcePath);

            if (Profile.IsEmpty)
            {
                Profile = Configuration.DirectoryProfileMap.GetOrDefault(sourcePath, NullArchiveNowProfile.Instance);
            }

            if (!Profile.IsValid(out string message))
            {
                throw new ValidationException($"Profile validation error!\n\n{message}");
            }

            ISearchFileProvider searchFileProvider;

            if (sourcePath.IsDirectory())
            {
                searchFileProvider = SearchFileProviderFactory.Build(
                    Profile.IgnoredDirectories,
                    Profile.IgnoredFiles);
            }
            else
            {
                searchFileProvider = new StaticSearchFileProvider();
            }

            ICollection<FileSystemInfo> entries;

            using (var performance = new PerformanceTester())
            {
                entries = searchFileProvider.GetEntries(sourcePath).ToList();

                _logger.Info($"Files found: {entries.Count}");
                _logger.Debug($"Files search duration: {performance.Result}");
            }

            OnInitialized(entries.Count);

            IArchiveEntryTransform entryTransform = new RelativePathTransform(sourcePath);
            IFileNameBuilderContext nameBuilderContext = new FileNameBuilderContext(sourcePath);
            IArchiveFilePathBuilder archiveFilePathBuilder = new ArchiveFilePathBuilder(
                sourcePath,
                Profile.FileNameBuilder,
                nameBuilderContext,
                _logger
                );
            IPasswordProvider passwordProvider = CreatePasswordProvider();

            IArchiveProvider archiveProvider;
            IArchiveResult result;

            try
            {
                using (archiveProvider = ArchiveProviderFactory.Build(Profile.ProviderName, archiveFilePathBuilder, entryTransform, passwordProvider))
                {
                    var archiveContext = new ArchiveContext(sourcePath, entries, archiveProvider);

                    result = ArchiveAsync(archiveContext, progressIndicator, cancelToken, pauseToken).Result;
                }
            }
            catch (Exception e)
            {
                throw new ArchiveNowException("An error occurred while creating archive!", e);
            }

            if (result.IsSuccess)
            {
                try
                {
                    IEnumerable<IAfterFinishedAction> actions = Profile.AfterFinishedActions;
                    if (Profile.UseDefaultActionPrecedence)
                    {
                        actions = actions.OrderBy(a => a.Precedence);
                    }

                    var inputPath = result.ArchivePath;

                    var progress = new Progress<AfterFinishedActionProgress>();
                    progress.ProgressChanged += ProgressOnProgressChanged;

                    var additionalPaths = new List<string>();

                    foreach (var action in actions)
                    {
                        OnActionExecuting(action);

                        action.Progress = progress;
                        var actionResult = action.Execute(new AfterFinishedActionContext(inputPath, additionalPaths));

                        OnActionExecuted(action, actionResult);

                        if (actionResult.IsSuccess.IsNotTrue())
                        {
                            ActionOnErrorDelay();

                            if (action.BreakIfError)
                            {
                                break;
                            }
                        }
                        else
                        {
                            inputPath = actionResult.OutputPath;
                            ActionDelay();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArchiveNowException("An error occurred while execution actions!", ex);
                }
            }
            else
            {
                CleanUp(archiveProvider.ArchiveFilePath);
            }

            OnFinished(result);
        }

        private IPasswordProvider CreatePasswordProvider()
        {
            IPasswordProvider passwordProvider = NullPasswordProvider.Instance;
            if (Profile.Password.HasValue())
            {
                if (Profile.UsePlainTextPasswords)
                {
                    passwordProvider = new PlainTextPasswordProvider(Profile.Password);
                }
                else
                {
                    passwordProvider = new SecureTextPasswordProvider(Profile.Password);
                }
            }

            return passwordProvider;
        }

        private void ProgressOnProgressChanged(object sender, AfterFinishedActionProgress e)
        { }

        private static void CleanUp(string archiveFilePath)
        {
            FileSystemExtensions.DeletePath(archiveFilePath);
        }

        private static uint GetSystemMaxPathLength(string driveLetter)
        {
            var info = DriveInformation.Get(driveLetter);

            return info.MaxPathLength;
        }

        private async Task<IArchiveResult> ArchiveAsync(ArchiveContext context,
           IArchiveNowProgress progressIndicator, CancellationToken cancelToken, PauseToken pauseToken)
        {
            EnsureArg.IsNotNull(context, nameof(context));
            EnsureArg.IsNotNull(context, nameof(progressIndicator));

            IArchiveProvider archiveProvider = context.Provider;
            
            var report = new ArchiveNowProgressReport(context.Entries.Count);

            archiveProvider.IsProgressIndeterminateChanged += (sender, isIndeterminate) =>
            {
                progressIndicator.IsIndeterminate = isIndeterminate;
            };

            archiveProvider.Finished += (sender, args) =>
            {
                report.Done();
                progressIndicator?.Report(report);
            };

            archiveProvider.FileCompressed += (sender, args) =>
            {
                report.Step();
                progressIndicator?.Report(report);
            };

            archiveProvider.BeginUpdate(context.SourcePath);

            using (var performance = new PerformanceTester())
            {
                if (!archiveProvider.IsBatchOnly)
                {
                    foreach (var entry in context.Entries)
                    {
                        Debug.WriteLine($"[{entry.FullName}] [{entry.FullName.Length}]");

                        report.CurrentEntry = entry;

                        if (cancelToken.IsCancellationRequested)
                        {
                            const string errorMessage = "Archiving has been canceled!";

                            _logger.Error(errorMessage);

                            archiveProvider.AbortUpdate();
                            return ArchiveResult.Abort(errorMessage);
                        }

                        if (pauseToken.IsPaused)
                        {
                            await pauseToken.WaitWhilePausedAsync();
                        }

                        if (TryAddEntryToArchive(archiveProvider, entry) == false)
                        {
                            throw new NotImplementedException("TODO: Polityka w przypadku błędu dodawania pliku do archiwum.");
                        }

                        report.Step();
                        progressIndicator?.Report(report);
                    }

                    report.Clear();
                    progressIndicator?.Report(report);
                }

                OnCommit(archiveProvider.ArchiveFilePath);

                progressIndicator.IsIndeterminate = true;
                progressIndicator?.Report(report);

                try
                {
                    archiveProvider.CommitUpdate(cancelToken);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Provider->CommitUpdate: {ex}");
                    return ArchiveResult.Fail(ex.Message);
                }

                if (cancelToken.IsCancellationRequested)
                {
                    const string errorMessage = "Archiving has been canceled!";

                    _logger.Error(errorMessage);

                    archiveProvider.AbortUpdate();
                    return ArchiveResult.Abort(errorMessage);
                }
                
                _logger.Info($"Time elapsed: {performance.Result}");

                return ArchiveResult.Success(performance.Result, archiveProvider.ArchiveFilePath);
            }
        }

        private bool TryAddEntryToArchive(IArchiveProvider archiveProvider, FileSystemInfo entry)
        {
            var path = entry.FullName;

            if (entry.IsPathTooLong())
            {
                Debug.WriteLine($"[{path}] - Path is too long!");
                return false;
            }

            try
            {
                if (entry.IsDirectory())
                {
                    OnDirectoryAdding(path);

                    archiveProvider.AddDirectory(path);

                    OnDirectoryAdded(path);
                }
                else
                {
                    OnFileAdding(path);

                    archiveProvider.Add(path);
                    FileProcessingDelay();

                    OnFileAdded(path);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex}");
                return false;
            }

            return true;
        }

        private static void TryHandleTooLongPath(string sourcePath)
        {
            Debug.WriteLine($"[{sourcePath}] [{sourcePath.Length}]");

            var sourcePathRoot = Path.GetPathRoot(sourcePath);
            var pathMaxLength = GetSystemMaxPathLength(sourcePathRoot);

            if (pathMaxLength > 260)
            {
                return;
            }

            if (PathHelper.IsLongPathsEnabled())
            {
                PathHelper.SetLongPathSupport();
            }
        }

        private static void Delay(TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
        }

        private static void ActionOnErrorDelay()
        {
            Delay(TimeSpan.FromSeconds(3));
        }

        /// <summary>
        /// Delay only for UI purpose.
        /// </summary>
        private static void ActionDelay()
        {
            Delay(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Delay only for UI purpose.
        /// </summary>
        private static void FileProcessingDelay()
        {
            //Delay(TimeSpan.FromMilliseconds(10));
        }

        private static IEnumerable<FileSystemInfo> Prepare(IEnumerable<FileSystemInfo> entries)
        {
            // At the beginning create first all the folders in the archive. This step may be
            // necessary for some ZIP libraries.
            return entries.OrderBy(x => x.IsDirectory());
        }
    }
}
