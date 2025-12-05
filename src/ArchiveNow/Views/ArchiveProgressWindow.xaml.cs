using ArchiveNow.Actions.Core;
using ArchiveNow.Service;
using ArchiveNow.Utils;
using ArchiveNow.Utils.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ArchiveNow.Views
{
    public partial class ArchiveProgressWindow : Window
    {
        private const string CloseText = "Close";
        private const string CancelText = "Cancel";
        private const string PauseText = "Pause";
        private const string ResumeText = "Resume";

        private readonly IArchiveNowService _service;
        private readonly string _path;
        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private readonly PauseTokenSource _pauseTokenSource = new PauseTokenSource();

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly DispatcherTimer _timer;

        public bool HasAnyError { get; set; }

        public bool IsFinished { get; set; }

        public bool CloseWindowOnSuccess { get; set; } = false;

        public ArchiveProgressWindow()
        {
            InitializeComponent();
        }

        public ArchiveProgressWindow(IArchiveNowService service, string path)
            : this()
        {
            _service = service;
            _service.Initialized += OnServiceInitialized;
            _service.FileAdded += OnServiceFileAdded;
            _service.DirectoryAdded += OnServiceDirectoryAdded;
            _service.Commit += OnServiceCommit;
            _service.Finished += OnServiceFinished;
            _service.ActionExecuting += OnActionExecuting;
            _service.ActionExecuted += OnActionExecuted;

            _path = path;

            Title = _service.IsUsingProfile
                ? $"Archiving... (using profile: \"{_service.Profile.Name}\")"
                : "Archiving...";


            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (!_stopwatch.IsRunning)
                return;

            var elapsed = _stopwatch.Elapsed;

            elapsedTimeTextBlock.Text = elapsed.ToString(@"hh\:mm\:ss");
        }

        private void OnActionExecuting(object sender, AfterFinishedActionEventArgs args)
        {
            OnUIUpdateDirectoryPathTextBox($"Executing action: {args.Action.Description}");
        }

        /// <summary>
        /// TODO: Dorobić inne kolory dla "Done" i "Error"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnActionExecuted(object sender, AfterFinishedActionEventArgs args)
        {
            var hasError = !args.Result.IsSuccess;
            if (hasError)
            {
                if (_service.Profile.BreakActionsIfError)
                {
                    args.Break = true;
                }

                if (HasAnyError.IsNotTrue())
                {
                    HasAnyError = true;
                }
            }

            OnUIUpdateDirectoryPathTextBox(args.Result.IsSuccess ? "Done." : "Error!");
            OnUIUpdateProgressBar(args.Result.IsSuccess);
        }

        private void OnServiceCommit(object sender, string path)
        {
            OnUIUpdateDirectoryPathTextBox("Creating output file...");
            OnUIUpdateFilePathTextBox(Path.GetFileName(path));
        }

        private void OnServiceFinished(object sender, IArchiveResult result)
        {
            IsFinished = true;
            HasAnyError = !result.IsSuccess;

            UpdateButtonVisibility(_pauseButton, false);
            UpdateButtonVisibility(_cancelButton, false);
            UpdateButtonVisibility(_closeButton, true);

            OnUIUpdateDirectoryPathTextBox(result.IsSuccess ? "Done." : $"Error! {result.Message}");
            OnUIUpdateProgressBar(result.IsSuccess);
        }

        private void OnServiceDirectoryAdded(object sender, string path)
        {
            OnUIUpdateDirectoryPathTextBox(path);
        }

        private void OnServiceFileAdded(object sender, string path)
        {
            OnUIUpdateFilePathTextBox(path);
        }

        private void OnUIThread(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        private void OnUIUpdateDirectoryPathTextBox(string path)
        {
            OnUIThread(() => directoryPathTextBox.Text = path);
        }

        private void UpdateButtonVisibility(Button button, bool isVisible)
        {
            OnUIThread(() =>
                button.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed
            );
        }

        private void OnUIUpdateProgressBar(bool isSuccess)
        {
            if (isSuccess)
            {
                return;
            }

            //OnUIThread(() => archivingProgressBar.UpdateDefaultStyle());
            OnUIThread(() => archivingProgressBar.Foreground = new SolidColorBrush { Color = Colors.Red });
        }

        private void OnUIUpdateFilePathTextBox(string path)
        {
            OnUIThread(() => filePathTextBox.Text = Path.GetFileName(path));
        }

        private void OnUIUpdateProgressBar(int entriesToProcess)
        {
            OnUIThread(() =>
            {
                archivingProgressBar.Value = 0;
                archivingProgressBar.Maximum = entriesToProcess;
            });
        }

        private void OnServiceInitialized(object sender, int entriesToProcess)
        {
            IsFinished = false;

            OnUIUpdateProgressBar(entriesToProcess);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            _cancelTokenSource.Cancel();

            _stopwatch.Stop();
            _timer.Stop();

            Close();
        }

        private void OnPauseButtonClick(object sender, RoutedEventArgs e)
        {
            _pauseTokenSource.IsPaused = _pauseTokenSource.IsPaused.Not();

            _pauseButton.Content = _pauseTokenSource.IsPaused ? ResumeText : PauseText;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var progressIndicator = new ArchiveNowProgress(OnReportProgress);

            _stopwatch.Start();
            _timer.Start();

            try
            {
                await ArchiveAsync(_path, progressIndicator, _cancelTokenSource.Token, _pauseTokenSource.Token);
            }
            catch (Exception ex)
            {
                var message = $"{ex.Message}\n\n{ex.InnerException?.Message}";

                MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _stopwatch.Stop();
                _timer.Stop();
            }

            if (HasAnyError.IsNotTrue())
            {
                if (CloseWindowOnSuccess)
                {
                    Close();
                }
                else
                {
                    //OnUIUpdateVisibilityPauseButton(false);
                }
            }
        }

        private async Task ArchiveAsync(string path, IArchiveNowProgress progress, CancellationToken cancelToken, PauseToken pauseToken)
        {
            await Task.Run(
                () =>
                {
                    _service.Archive(path, progress, cancelToken, pauseToken);
                }, cancelToken);
        }

        private void OnReportProgress(ArchiveNowProgressReport value)
        {
            if (value.IsIndeterminate)
            {
                archivingProgressBar.IsIndeterminate = true;
            }

            archivingProgressBar.Value = value.ProcessedEntriesCount;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs args)
        {
            Close();
        }
    }
}
