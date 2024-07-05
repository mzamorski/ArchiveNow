using Humanizer;

namespace ArchiveNow.Configuration.Profiles
{
    public class IncorrectArchiveNowProfile : NullArchiveNowProfile
    {
        public string ErrorMessage { get; }

        public string ProfileFileName { get; }

        public override string Name =>
            $">> Incorrect profile file: \"{ProfileFileName}\"! {ErrorMessage}"
                .Truncate(200, "...");

        public IncorrectArchiveNowProfile(string errorMessage, string profileFileName)
        {
            ErrorMessage = errorMessage;
            ProfileFileName = profileFileName;
        }

        public override bool IsValid(out string message)
        {
            message = ErrorMessage;

            return false;
        }
    }
}