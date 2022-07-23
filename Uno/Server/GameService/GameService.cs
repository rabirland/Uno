using Uno.Server.Models.Game;

namespace Uno.Server.GameService;

public class GameService
{
    private readonly List<GameEntry> games = new List<GameEntry>();

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
}
