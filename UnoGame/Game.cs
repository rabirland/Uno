using UnoGame.Decks;
using UnoGame.Timer;

namespace UnoGame;

public class Game
{
    private readonly GameSettings settings;
    private readonly GameTimer Timer = new GameTimer();
    private readonly object gameLock = new object();

    private List<CardFace> playedCards = new List<CardFace>();
    private int pullCounter = 1;

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
    public event Action OnStateChange = () => { };

    public Game(GameSettings settings, IEnumerable<string> playerIds)
    {
        this.settings = settings;

        this.Deck = new InfiniteDeck(); // TODO: Get from settings

        this.players = playerIds
            .Select(p => new Player(p) { Active = true, FinishedNumber = 0 })
            .ToArray();

        foreach (var player in this.players)
        {
            for (int i = 0; i < this.settings.StartCardCount; i++)
            {
                player.AddCard(this.Deck.Pull());
            }
        }

        // Ensure the starter card has a color and a number
        var starterCard = this.Deck.Pull();
        while (starterCard.Color == CardColor.Colorless || starterCard.Type.IsNumerical() == false)
        {
            this.Deck.Push(starterCard);
            starterCard = this.Deck.Pull();
        }

        this.playedCards.Add(starterCard);
        this.ActiveColor = starterCard.Color;
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
    /// The last picked card color.
    /// </summary>
    public CardColor ActiveColor { get; private set; }

    /// <summary>
    /// The current phase of the round.
    /// </summary>
    public RoundPhase RoundPhase { get; private set; }

    /// <summary>
    /// The list of players in the game.
    /// </summary>
    public IEnumerable<Player> Players => this.players;

    /// <summary>
    /// The cards those was dropped by the players.
    /// </summary>
    public IEnumerable<CardFace> PlayedCards => this.playedCards;

    /// <summary>
    /// Plays a card from the actual player.
    /// </summary>
    /// <param name="playerId">The id of the player that tries to drop the card.</param>
    /// <param name="cardFace">The face of the card to drop.</param>
    /// <param name="count">The amount of cards to drop.</param>
    public bool PlayCard(CardFace cardFace, int count)
    {
        lock (this.gameLock)
        {
            var lastlyPlayedCard = LastCard;

            var colorMatch = cardFace.Color == CardColor.Colorless || cardFace.Color == this.ActiveColor;
            var typeMatch = cardFace.Type == lastlyPlayedCard.Type;

            if (colorMatch == false && typeMatch == false)
            {
                return false;
            }

            // If +X cards began to be stacked, only +X cards can be dropped.
            var isDrawCard = cardFace.Type.IsDraw();
            if (this.pullCounter > 1 && isDrawCard == false)
            {
                return false;
            }

            if (this.RoundPhase != RoundPhase.Card)
            {
                throw new Exception("Invalid action for the current round phase");
            }

            var card = CardMetadata.ValidCards.FirstOrDefault(c => c.Face == cardFace);

            var player = this.players.First(p => p.Id == this.CurrentPlayerId);
            if (player.RemoveCard(cardFace, count))
            {
                var droppedCards = Enumerable.Repeat(cardFace, count);
                this.playedCards.AddRange(droppedCards);

                // Switching color
                if (cardFace.Color != CardColor.Colorless)
                {
                    this.ActiveColor = cardFace.Color;
                }
                else if (card.CurrentAction.HasFlag(CurrentPlayerAction.PickColor) == false)
                {
                    throw new Exception("A colorless card must have a pick color \"current round\" flag");
                }

                // Immediate action
                switch(card.ImmediateAction)
                {
                    case ImmediateAction.None: break;
                    case ImmediateAction.ReverseOrder: this.gameDirectionRightHand = !this.gameDirectionRightHand; break;
                    default: throw new Exception("Invalid \"immediate\" action");
                }

                // Current round action
                AdvancePhaseByCurrentAction(card.CurrentAction);
                ApplyNextPlayerEffects(card.NextPlayerAction);
                return true;
                
            }
            else
            {
                throw new Exception("Invalid action");
            }
        }
    }

    /// <summary>
    /// Picks a player by the currently active player.
    /// </summary>
    /// <param name="playerId">The id of the other player.</param>
    public void PickPlayer(string playerId)
    {
        lock (this.gameLock)
        {
            if (this.RoundPhase != RoundPhase.Player)
            {
                throw new Exception("Invalid round phase");
            }

            var otherPlayer = this.players.FirstOrDefault(p => p.Id == playerId);
            if (otherPlayer == default)
            {
                throw new Exception("Invalid player id");
            }

            var lastPlayedCard = LastCard;
            var card = CardMetadata.ValidCards.First(c => c.Face == lastPlayedCard);

            // Do the action
            if (card.CurrentAction.HasFlag(CurrentPlayerAction.SwapHandDeckWithPlayer))
            {
                // If the player selected himself, Do not swap cards
                if (otherPlayer.Id != this.CurrentPlayerId)
                {
                    var currentPlayer = this.players[this.currentPlayerIndex];
                    currentPlayer.SwapCardsWith(otherPlayer);
                }
            }
            else
            {
                throw new Exception("Invalid action for the last card");
            }

            AdvancePhaseByCurrentAction(card.CurrentAction);
        }
    }

    private void AdvancePhaseByCurrentAction(CurrentPlayerAction action)
    {
        if (this.RoundPhase == RoundPhase.Card)
        {
            if (action.HasFlag(CurrentPlayerAction.SwapHandDeckWithPlayer))
            {
                this.RoundPhase = RoundPhase.Player;
            }
            else if (action.HasFlag(CurrentPlayerAction.PickColor))
            {
                this.RoundPhase = RoundPhase.Color;
            }
            else
            {
                RoundDone();
            }
        }
        else if (this.RoundPhase == RoundPhase.Player)
        {
            if (action.HasFlag(CurrentPlayerAction.PickColor))
            {
                this.RoundPhase = RoundPhase.Color;
            }
            else
            {
                RoundDone();
            }
        }
        else if (this.RoundPhase == RoundPhase.Color)
        {
            RoundDone();
        }
        else
        {
            throw new Exception("Invalid round phase");
        }
    }

    /// <summary>
    /// Pick the currently active color.
    /// </summary>
    /// <param name="color">The card color.</param>
    public void PickColor(CardColor color)
    {
        lock (this.gameLock)
        {
            if (this.RoundPhase != RoundPhase.Color)
            {
                throw new Exception("Invalid round phase");
            }

            if (color.IsChromatic() == false)
            {
                throw new Exception("Invalid color");
            }

            var lastPlayedCard = LastCard;
            var card = CardMetadata.ValidCards.First(c => c.Face == lastPlayedCard);

            // Do the action
            if (card.CurrentAction.HasFlag(CurrentPlayerAction.PickColor))
            {
                this.ActiveColor = color;
            }
            else
            {
                throw new Exception("Invalid action for the last card");
            }

            AdvancePhaseByCurrentAction(card.CurrentAction);
        }
    }

    /// <summary>
    /// Pulls a card for the actual player.
    /// </summary>
    /// <param name="playerId"></param>
    public void PullCard()
    {
        var player = this.players.First(p => p.Id == this.CurrentPlayerId);

        for (int i = 0; i < this.pullCounter; i++)
        {
            player.AddCard(this.Deck.Pull());
        }

        this.pullCounter = 1;

        this.RoundDone();
    }

    private CardFace LastCard => this.playedCards[this.playedCards.Count - 1];

    /// <summary>
    /// Calls <see cref="AdvancePlayer"/> and fires <see cref="OnStateChange"/>
    /// with the new state of the game.
    /// </summary>
    private void RoundDone()
    {
        var player = this.players[this.currentPlayerIndex];

        if (player.Cards.Any(c => c.Value > 0) == false)
        {
            var previousFinisher = this.players.Select(p => p.FinishedNumber).Max();
            player.FinishedNumber = previousFinisher + 1;
        }

        AdvancePlayer();
        this.RoundPhase = RoundPhase.Card;

        this.OnStateChange();
    }

    /// <summary>
    /// Advances the active player to the next according to the direction of the game.
    /// </summary>
    private void AdvancePlayer()
    {
        var playerCount = this.players.Count();

        Player player;
        do
        {
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

            player = this.players[this.currentPlayerIndex];
        } while (player.FinishedNumber != 0 && player.Active != true);
    }

    /// <summary>
    /// Applies the effects from cards that affects the next-in-order player instead the one that played the card.
    /// </summary>
    private void ApplyNextPlayerEffects(NextPlayerAction action)
    {
        if (action == NextPlayerAction.None)
        {
            // Do nothing
        }
        else if (action == NextPlayerAction.Skip)
        {
            AdvancePlayer();
        }
        else if (action == NextPlayerAction.Draw2)
        {
            if (this.settings.StackDraw2)
            {
                this.BeginStackingDrawCards();
                this.pullCounter += 2;
            }
            else
            {
                var player = this.players[this.currentPlayerIndex];
                player.AddCard(this.Deck.Pull());
                player.AddCard(this.Deck.Pull());
                AdvancePlayer();
            }
        }
        else if (action == NextPlayerAction.Draw4)
        {
            if (this.settings.StackDraw4)
            {
                this.BeginStackingDrawCards();
                this.pullCounter += 4;
            }
            else
            {
                var player = this.players[this.currentPlayerIndex];
                player.AddCard(this.Deck.Pull());
                player.AddCard(this.Deck.Pull());
                player.AddCard(this.Deck.Pull());
                player.AddCard(this.Deck.Pull());
                AdvancePlayer();
            }
        }
        else
        {
            throw new Exception("Invalid action for \"Next player\"");
        }
    }

    /// <summary>
    /// Prepares the pull counter for stacking +X cards.
    /// </summary>
    private void BeginStackingDrawCards()
    {
        if (this.pullCounter == 1)
        {
            this.pullCounter = 0;
        }
    }
}
