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

    public void PlayerConnected(string token)
    {
        var index = this.players.FindIndex(p => p.Token == token);
        if (index < 0)
        {
            throw new Exception("Invalid token");
        }

        if (this.Game != null)
        {
            var gamePlayer = this.Game.Players.First(p => p.Id == token);
            gamePlayer.Active = true;
        }
    }

    public void PlayerDisconnected(string token)
    {
        var index = this.players.FindIndex(p => p.Token == token);

        if (index < 0)
        {
            return;
        }

        if (this.Status == GameStatus.InLobby)
        {
            this.players.RemoveAt(index);
        }
        //else if (this.Status == GameStatus.Running)
        //{
        //    if (this.Game == null)
        //    {
        //        throw new Exception("Invalid state");
        //    }

        //    var player = this.players[index];
        //    var gamePlayer = this.Game.Players.First(p => p.Id == player.Token);
        //    gamePlayer.Active = false;

        //    var notFinished = this.Game.Players.Where(p => p.Active).Count();
        //    if (notFinished <= 1)
        //    {
        //        this.Game.Finish();
        //    }
        //}
    }

    public void StartGame()
    {
        if (this.CanStart == false)
        {
            return;
        }

        this.Game = new UnoGame.Game(UnoGame.GameSettings.Default, this.players.Select(p => p.Token));
        this.Status = GameStatus.Running;
        this.Game.OnGameFinish += this.OnGameFinished;
    }

    private void OnGameFinished()
    {
        if (this.Game == null)
        {
            throw new Exception("Invalid event");
        }

        this.Status = GameStatus.Finished;
        this.Game.OnGameFinish -= this.OnGameFinished;
    }
}
