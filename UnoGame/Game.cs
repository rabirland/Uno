using UnoGame.Decks;
using UnoGame.Timer;

namespace UnoGame;

public class Game
{
    private List<CardFace> playedCards = new List<CardFace>();
    private readonly GameSettings settings;
    private readonly GameTimer Timer = new GameTimer();
    private readonly object playerLock = new object();

    /// <summary>
    /// The list of all players in the game.
    /// </summary>
    private readonly Player[] players;

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

    public Game(GameSettings settings, IEnumerable<string> playerIds)
    {
        this.settings = settings;

        this.Deck = new InfiniteDeck(); // TODO: Get from settings

        this.players = playerIds
            .Select(p => new Player(p))
            .ToArray();

        foreach (var player in this.players)
        {
            for (int i = 0; i < this.settings.StartCardCount; i++)
            {
                player.AddCard(this.Deck.Pull());
            }
        }
    }

    /// <summary>
    /// The id of the player whose round is ongoing.
    /// </summary>
    public string CurrentPlayerId => this.players[currentPlayerIndex].Id;

    /// <summary>
    /// The deck to pull cards from.
    /// </summary>
    public IDeck Deck { get; }

    /// <summary>
    /// The list of players in the game.
    /// </summary>
    public IEnumerable<Player> Players => this.players;

    /// <summary>
    /// The cards those was dropped by the players.
    /// </summary>
    public IEnumerable<CardFace> PlayedCards => this.playedCards;

    /// <summary>
    /// The player tries to drop a card.
    /// </summary>
    /// <param name="playerId">The id of the player that tries to drop the card.</param>
    /// <param name="cardFace">The face of the card to drop.</param>
    /// <param name="count">The amount of cards to drop.</param>
    public void DropCard(string playerId, CardFace cardFace, int count)
    {
        lock (this.playerLock)
        {
            if (!IsCurrentPlayer(playerId))
            {
                throw new Exception("Not the current player to play");
            }

            var player = this.players.First(p => p.Id == playerId);
            if (player.RemoveCard(cardFace, count))
            {
                var droppedCards = Enumerable.Repeat(cardFace, count);
                this.playedCards.AddRange(droppedCards);
            }

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
    /// Checks if the given <paramref name="playerId"/> is the id of the current active player.
    /// </summary>
    /// <param name="playerId">The player name.</param>
    /// <returns><see langword="true"/> if the current player's name is <paramref name="playerId"/>.</returns>
    private bool IsCurrentPlayer(string playerId)
    {
        lock (this.playerLock)
        {
            return this.CurrentPlayerId == playerId;
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

                return new PlayerState(p.Id, cardCounts);
            });

        return new GameState(this.CurrentPlayerId, players);
    }
}
