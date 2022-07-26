using Uno.Server.Annotation;
using Uno.Server.Models.Game;

namespace Uno.Server.GameService;

[Service(AsSingleton = true)]
public class GameService : IGameService
{
    private readonly List<GameEntry> games = new List<GameEntry>();

    /// <inheritdoc/>
    public GameCreateResult Create()
    {
        var adminToken = TokenCreator.CreateRandomToken(32);
        var gameToken = TokenCreator.CreateRandomToken(16, true);

        var entry = new GameEntry(gameToken, adminToken);
        games.Add(entry);

        return new GameCreateResult(gameToken, adminToken);
    }

    public GameEntry? FindGameByPlayerToken(string token)
    {
        var entry = this.games.FirstOrDefault(e => e.Players.Any(p => p.Token == token));
        return entry;
    }

    public bool JoinPlayerToGame(string gameId, GamePlayer player)
    {
        var game = this.games.FirstOrDefault(g => g.GameId == gameId);

        if (game == default)
        {
            throw new Exception("Invalid game id");
        }

        return game.TryAddPlayer(player);
    }
}
