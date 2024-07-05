using System.Threading;
using System.Threading.Tasks;

namespace ArchiveNow.Utils.Threading
{
    /// <summary>
    /// Implementation of PauseTokenSource pattern based on the blog post: 
    /// http://blogs.msdn.com/b/pfxteam/archive/2013/01/13/cooperatively-pausing-async-methods.aspx 
    /// </summary>
    public class PauseTokenSource
    {
        private TaskCompletionSource<bool> _paused;

        internal static readonly Task CompletedTask = Task.FromResult(true);

        public bool IsPaused
        {
            get
            {
                return _paused != null;
            }

            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(
                        ref _paused, new TaskCompletionSource<bool>(), null);
                }
                else
                {
                    while (true)
                    {
                        var tcs = _paused;
                        if (tcs == null)
                        {
                            return;
                        }

                        if (Interlocked.CompareExchange(ref _paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }

        public PauseToken Token => new PauseToken(this);

        internal Task WaitWhilePausedAsync()
        {
            var cur = _paused;

            return (cur != null) 
                ? cur.Task 
                : CompletedTask;
        }
    }
}
