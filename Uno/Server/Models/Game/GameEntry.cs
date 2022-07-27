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
    public GameStatus Status { get; private set; }

    /// <summary>
    /// The UNO game.
    /// </summary>
    public UnoGame.Game? Game { get; private set; }

    /// <summary>
    /// The list of players in the game or waiting for the game.
    /// </summary>
    public IEnumerable<GamePlayer> Players => this.players;

    /// <summary>
    /// The admin player or <see langword="null"/> if haven't joined yet.
    /// </summary>
    public GamePlayer? AdminPlayer => this.players.FirstOrDefault(p => p.Token == this.AdminPlayerToken);

    /// <summary>
    /// Whether the game can be started.
    /// </summary>
    public bool CanStart => this.Status == GameStatus.InLobby && this.players.Count >= 2;

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

        if (players.Any(p => p.PlayerName == player.PlayerName))
        {
            return false;
        }

        this.players.Add(player);
        return true;
    }

    public void RemovePlayer(string token)
    {
        if (this.Status != GameStatus.InLobby)
        {
            return;
        }

        var index = this.players.FindIndex(p => p.Token == token);
        if (index >= 0)
        {
            this.players.RemoveAt(index);
        }
    }

    public void StartGame()
    {
        if (this.CanStart == false)
        {
            return;
        }

        this.Game = new UnoGame.Game(this.players.Select(p => p.PlayerName));
        this.Status = GameStatus.Running;
    }
}
