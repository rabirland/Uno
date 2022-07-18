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
    /// Whether the game is going right hand (<see langword="true"/>) or left hand (<see langword="false"/>) direction.
    /// </summary>
    private bool gameDirectionRightHand = true;

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

            // TODO: Player action

            RoundDone();
        }
    }

    /// <summary>
    /// Calls <see cref="AdvancePlayer"/> and fires <see cref="OnStateChange"/>
    /// with the new state of the game.
    /// </summary>
    private void RoundDone()
    {
        AdvancePlayer();

        var state = ConstructCurrentState();
        this.OnStateChange(state);
    }

    /// <summary>
    /// Advances the active player to the next according to the direction of the game.
    /// </summary>
    private void AdvancePlayer()
    {
        var playerCount = this.players.Count();

        if (this.gameDirectionRightHand)
        {
            this.currentPlayerIndex++;

            if (this.currentPlayerIndex >= playerCount)
            {
                this.currentPlayerIndex = 0;
            }
        }
        else
        {
            this.currentPlayerIndex--;

            if (this.currentPlayerIndex < 0)
            {
                this.currentPlayerIndex = playerCount - 1;
            }
        }
    }

    /// <summary>
    /// Checks if the given <paramref name="playerName"/> is the name of the current active player.
    /// </summary>
    /// <param name="playerName">The player name.</param>
    /// <returns><see langword="true"/> if the current player's name is <paramref name="playerName"/>.</returns>
    private bool IsCurrentPlayer(string playerName)
    {
        lock (this.playerLock)
        {
            return this.CurrentPlayer == playerName;
        }
    }

    /// <summary>
    /// Constructs a gamestate.
    /// </summary>
    /// <returns>The actual state of the game.</returns>
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
