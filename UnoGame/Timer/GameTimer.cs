namespace UnoGame.Timer
{
    public class GameTimer
    {
        /// <summary>
        /// If not <see langword="null"/>, the objects of the current timeout awaiter.
        /// </summary>
        private TimeoutAwaiter? timeoutAwaiter;

        public void Start(int timeoutMs, Action onTimeout)
        {
            if (this.timeoutAwaiter != null)
            {
                throw new InvalidOperationException("There is already a timer running, cancel it before.");
            }

            var tokenSource = new CancellationTokenSource();
            var timerTask = TimerFunction(timeoutMs, onTimeout, tokenSource.Token);

            this.timeoutAwaiter = new TimeoutAwaiter(timerTask, tokenSource);
        }

        public void Cancel()
        {
            if (this.timeoutAwaiter == null)
            {
                throw new InvalidOperationException("There is no active timeout awaiter");
            }

            this.timeoutAwaiter.Value.TokenSource.Cancel();
            this.timeoutAwaiter.Value.Task.Wait();

            this.timeoutAwaiter = null;
        }

        private static async Task TimerFunction(int timeoutMs, Action onTimeout, CancellationToken token)
        {
            await Task.Delay(timeoutMs, token);

            if (token.IsCancellationRequested == false)
            {
                onTimeout();
            }
        }

        private readonly record struct TimeoutAwaiter(Task Task, CancellationTokenSource TokenSource);
    }
}
