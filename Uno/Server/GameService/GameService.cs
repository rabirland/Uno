using Uno.Server.Annotation;
using Uno.Server.Models.Game;

namespace Uno.Server.GameService;

[Service(AsSingleton = true)]
public class GameService : IGameService, IDisposable
{
    private readonly Task cleanupTask;
    private readonly CancellationTokenSource cleanupTokenSource = new CancellationTokenSource();
    private readonly List<GameEntry> games = new List<GameEntry>();
    private readonly object locker = new object();

    public GameService()
    {
        this.cleanupTask = Task.Factory.StartNew(this.CleanupRoutine, TaskCreationOptions.LongRunning);
    }

    /// <inheritdoc/>
    public GameCreateResult Create()
    {
        lock (this.locker)
        {
            var adminToken = TokenCreator.CreateRandomToken(32);
            var gameToken = TokenCreator.CreateRandomToken(16, true);

            var entry = new GameEntry(gameToken, adminToken);
            games.Add(entry);

            return new GameCreateResult(gameToken, adminToken);
        }
    }

    /// <inheritdoc/>
    public GameEntry? FindGameByPlayerToken(string token)
    {
        lock (this.locker)
        {
            var entry = this.games.FirstOrDefault(e => e.Players.Any(p => p.Token == token));
            return entry;
        }
    }

    /// <inheritdoc/>
    public GameEntry? GetGame(string gameId)
    {
        lock (this.locker)
        {
            return this.games.FirstOrDefault(g => g.GameId == gameId);
        }
    }

    public void Dispose()
    {
        this.cleanupTokenSource.Cancel();
        this.cleanupTask.Wait();
    }

    private void CleanupRoutine()
    {
        while (this.cleanupTokenSource.IsCancellationRequested == false)
        {
            lock (this.locker)
            {
                int removeAt = -1;
                for (int i = 0; i < this.games.Count; i++)
                {
                    var gameEntry = this.games[i];

                    // If all the players disconnected from the lobby.
                    if (gameEntry.Players.Count() == 0)
                    {
                        removeAt = i;
                        break;
                    }

                    // Per-lobby cleanup should happen before this line. Everything after this line is in-game cleanup.
                    if (gameEntry.Game == null)
                    {
                        continue;
                    }

                    var activePlayers = gameEntry.Game.Players.Where(p => p.Active).Count();
                    if (activePlayers <= 1)
                    {
                        removeAt = i;
                        break;
                    }
                }

                if (removeAt >= 0)
                {
                    this.games.RemoveAt(removeAt);
                }
            }

            Thread.Sleep(10000);
        }
    }
}
