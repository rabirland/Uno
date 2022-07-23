using Uno.Server.Annotation;
using Uno.Server.Models.Game;

namespace Uno.Server.GameService;

[Service(AsSingleton = true)]
public class GameService : IGameService
{
    private readonly List<GameEntry> games = new List<GameEntry>();

    /// <inheritdoc/>
    public void Create(
        string gameName,
        IEnumerable<GamePlayer> players,
        string adminPlayerName)
    {
        var unoGame = new UnoGame.Game(players.Select(p => p.PlayerName));
        var entry = new GameEntry(
            gameName,
            unoGame,
            players.ToArray(),
            adminPlayerName);

        games.Add(entry);
    }

    public GameEntry? FindGameByPlayerToken(string token)
    {
        var entry = this.games.FirstOrDefault(e => e.Players.Any(p => p.Token == token));
        return entry;
    }
}
