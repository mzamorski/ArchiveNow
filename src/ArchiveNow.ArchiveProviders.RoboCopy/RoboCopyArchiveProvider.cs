

//using RoboSharp;

namespace ArchiveNow.Providers.RoboCopy
{
    //public class RoboCopyArchiveProvider : ArchiveProviderBase
    //{
    //    private readonly RoboCommand _backup;

    //    public RoboCopyArchiveProvider(IArchiveFilePathBuilder pathBuilder)
    //        : base(pathBuilder)
    //    {
    //        _backup = new RoboCommand();

    //        _backup.OnFileProcessed += OnFileProcessed;
    //        _backup.OnCommandCompleted += OnCommandCompleted;
    //        _backup.OnCopyProgressChanged += OnCopyProgressChanged;

    //        _backup.CopyOptions.Source = base.ArchiveFilePath;
    //        _backup.CopyOptions.Destination = @"F:\TEMP";
    //    }

    //    private void OnCopyProgressChanged(object sender, CopyProgressEventArgs copyProgressEventArgs)
    //    {

    //    }

    //    private void OnCommandCompleted(object sender, RoboCommandCompletedEventArgs roboCommandCompletedEventArgs)
    //    {

    //    }

    //    private void OnFileProcessed(object sender, FileProcessedEventArgs fileProcessedEventArgs)
    //    {
    //        OnFileCompressed();
    //    }

    //    public override string FileExtension { get; }

    //    public override void AddDirectory(string fullName)
    //    {

    //    }

    //    public override void Add(string fullName)
    //    {

    //    }

    //    public override void BeginUpdate()
    //    {
    //        _backup.Start();
    //    }

    //    public override void CommitUpdate()
    //    {

    //    }

    //    public override void AbortUpdate()
    //    {
    //        _backup.Stop();
    //    }
    //}

}
