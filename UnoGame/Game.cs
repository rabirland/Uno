using UnoGame.Timer;

namespace UnoGame;

public class Game
{
    private readonly GameTimer Timer = new GameTimer();

    private object playerLock = new object();

    /// <summary>
    /// The list of all players in the game.
    /// </summary>
    private Player[] players;

    /// <summary>
    /// The array index of the current player.
    /// </summary>
    private int currentPlayerIndex;

    /// <summary>
    /// Fired when the state of the game changes.
    /// </summary>
    public event Action<GameState> OnStateChange = _ => { };

    public Game(IEnumerable<string> playerNames)
    {
        this.players = playerNames
            .Select(p => new Player(p))
            .ToArray();
    }

    public string CurrentPlayer => this.players[currentPlayerIndex].Name;

    /// <summary>
    /// The player tries to drop a card.
    /// </summary>
    /// <param name="playerName">The name of the player that tries to drop the card.</param>
    /// <param name="cardFace">The face of the card to drop.</param>
    public void DropCard(string playerName, CardFace cardFace)
    {
        lock (this.playerLock)
        {
            if (!IsCurrentPlayer(playerName))
            {
                throw new Exception("Not the current player to play");
            }

            var player = this.players.First(p => p.Name == playerName);
            player.RemoveCard(cardFace);
        }
    }

    private bool IsCurrentPlayer(string playerName)
    {
        lock (this.playerLock)
        {
            return this.CurrentPlayer == playerName;
        }
    }

    private GameState ConstructCurrentState()
    {
        var players = this.players
            .Select(p =>
            {
                var cardCounts = p
                    .Cards
                    .Where(c => c.Value > 0)
                    .Select(c => new PlayerCardCount(c.Key, c.Value));

                return new PlayerState(p.Name, cardCounts);
            });

        return new GameState(this.CurrentPlayer, players);
    }
}
