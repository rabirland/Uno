namespace Uno.Server.Models.Game;

public class GameEntry
{
    private readonly List<GamePlayer> players = new List<GamePlayer>();

    public GameEntry(string gameId, string adminToken)
    {
        this.GameId = gameId;
        this.Status = GameStatus.InLobby;
        this.AdminPlayerToken = adminToken;
    }

    /// <summary>
    /// The id of the game.
    /// </summary>
    public string GameId { get; }

    /// <summary>
    /// The status of the game.
    /// </summary>
    public GameStatus Status { get; }

    /// <summary>
    /// The UNO game.
    /// </summary>
    public UnoGame.Game? Game { get; }

    /// <summary>
    /// The list of players in the game or waiting for the game.
    /// </summary>
    public IEnumerable<GamePlayer> Players => this.players;

    /// <summary>
    /// The token of the admin player.
    /// </summary>
    public string AdminPlayerToken { get; }

    /// <summary>
    /// Tries to add a player to the game. New players can only join during <see cref="GameStatus.InLobby"/> status.
    /// </summary>
    /// <param name="player">The player to add.</param>
    /// <returns><see langword="true"/> if the palyer is added.</returns>
    public bool TryAddPlayer(GamePlayer player)
    {
        if (this.Status != GameStatus.InLobby)
        {
            return false;
        }

        if (players.Any(p => p.Token == player.Token))
        {
            return false;
        }

        this.players.Add(player);
        return true;
    }
}
