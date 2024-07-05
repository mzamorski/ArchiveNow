using System.Threading.Tasks;

namespace ArchiveNow.Utils.Threading
{
    public struct PauseToken
    {
        private readonly PauseTokenSource _source;

        internal PauseToken(PauseTokenSource source)
        {
            _source = source;
        }

        public bool IsPaused => (_source != null) && _source.IsPaused;

        public Task WaitWhilePausedAsync()
        {
            return IsPaused 
                ? _source.WaitWhilePausedAsync() 
                : PauseTokenSource.CompletedTask;
        }
    }
}