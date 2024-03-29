﻿using UnoGame.Decks;
using UnoGame.Timer;

namespace UnoGame;

public class Game
{
    private readonly GameSettings settings;
    private readonly GameTimer Timer = new GameTimer();

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

    /// <summary>
    /// Fired when the game has finished.
    /// </summary>
    public event Action OnGameFinish = () => { };

    /// <summary>
    /// The current amount of stacked draw X cards.
    /// </summary>
    public int DrawCardCounter => pullCounter;

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
    /// Whether the direction of the game is clockwise.
    /// </summary>
    public bool DirectionClockwise => this.gameDirectionRightHand;

    /// <summary>
    /// Whether the game has ended.
    /// </summary>
    public bool GameFinished { get; private set; }

    private CardFace LastCard => this.playedCards[this.playedCards.Count - 1];

    /// <summary>
    /// Plays a card from the actual player.
    /// </summary>
    /// <param name="playerId">The id of the player that tries to drop the card.</param>
    /// <param name="cardFace">The face of the card to drop.</param>
    /// <param name="count">The amount of cards to drop.</param>
    public bool PlayCard(CardFace cardFace, int count)
    {
        if (this.GameFinished)
        {
            return false;
        }

        var lastlyPlayedCard = LastCard;

        var colorMatch = cardFace.Color == CardColor.Colorless || cardFace.Color == this.ActiveColor;
        var typeMatch = cardFace.Type == lastlyPlayedCard.Type;

        if (colorMatch == false && typeMatch == false)
        {
            return false;
        }

        // Handle dropping on a previously dropped PLUS card that hasn't "redeemed" yet
        if (this.pullCounter > 1)
        {
            // Disallow every card if stacking on +4 is not allowed
            if (lastlyPlayedCard.Type == CardType.Plus4 && settings.StackDraw4 == false)
            {
                return false;
            }

            // Disallow every card if stacking on +2 is not allowed
            if (lastlyPlayedCard.Type == CardType.Plus2 && settings.StackDraw2 == false)
            {
                return false;
            }

            // On a non-redeemed plus card, only plus cards can be dropped (unless the previous checks disallowed it)
            var isDrawCard = cardFace.Type.IsDraw();
            if (isDrawCard == false)
            {
                return false;
            }
        }

        if (this.RoundPhase != RoundPhase.Card)
        {
            return false;
        }

        var player = this.players.First(p => p.Id == this.CurrentPlayerId);
        if (player.RemoveCard(cardFace, count) == false)
        {
            return false;
        }

        var card = CardMetadata.ValidCards.First(c => c.Face == cardFace);
        
        var newDroppedCards = Enumerable.Repeat(cardFace, count);
        this.playedCards.AddRange(newDroppedCards);

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

        this.ApplyNextPlayerEffects(card.NextPlayerAction, count);
        this.PhaseDone(card.CurrentAction);
                
        return true;
    }

    /// <summary>
    /// Picks a player by the currently active player.
    /// </summary>
    /// <param name="playerId">The id of the other player.</param>
    public void PickPlayer(string playerId)
    {
        if (this.GameFinished)
        {
            return;
        }

        if (this.RoundPhase != RoundPhase.Player)
        {
            throw new Exception("Invalid round phase");
        }

        var otherPlayer = this.players.FirstOrDefault(p => p.Id == playerId);
        if (otherPlayer == default)
        {
            throw new Exception("Invalid player id");
        }

        // Can not select an already finished player
        if (otherPlayer.FinishedNumber != 0)
        {
            return;
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

        PhaseDone(card.CurrentAction);
    }

    /// <summary>
    /// Advances the round phase
    /// </summary>
    /// <param name="action">The action oif the card that is currently played.</param>
    private void PhaseDone(CurrentPlayerAction action)
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
                this.RoundDone();
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
                this.RoundDone();
            }
        }
        else if (this.RoundPhase == RoundPhase.Color)
        {
            this.RoundDone();
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
        // Validate if a valid action
        if (this.GameFinished)
        {
            return;
        }

        if (this.RoundPhase != RoundPhase.Color)
        {
            throw new Exception("Invalid round phase");
        }

        if (color.IsChromatic() == false)
        {
            throw new Exception("Invalid color");
        }

        var lastPlayedCard = LastCard;

        // Check if a card is a valid card.
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

        // Override the color of the last card on the player card list.
        this.playedCards[^1] = new CardFace(lastPlayedCard.Type, color);

        this.PhaseDone(card.CurrentAction);
    }

    /// <summary>
    /// Pulls a card for the actual player.
    /// </summary>
    /// <param name="playerId"></param>
    public void PullCard()
    {
        if (this.GameFinished)
        {
            return;
        }

        var player = this.players.First(p => p.Id == this.CurrentPlayerId);

        for (int i = 0; i < this.pullCounter; i++)
        {
            player.AddCard(this.Deck.Pull());
        }

        this.pullCounter = 1;

        this.RoundDone();
    }

    /// <summary>
    /// Forecefully finishes a game.
    /// </summary>
    public void Finish()
    {
        var previousFinisher = this.players.Select(p => p.FinishedNumber).Max();
        var notFinishedPlayers = players.Where(p => p.FinishedNumber == 0);
        previousFinisher++;

        foreach (var player in notFinishedPlayers)
        {
            player.FinishedNumber = previousFinisher;
            previousFinisher++;
        }

        this.GameFinished = true;
        this.OnGameFinish();
        return;
    }

    /// <summary>
    /// Calls <see cref="AdvancePlayer"/> and fires <see cref="OnStateChange"/>
    /// with the new state of the game.
    /// </summary>
    private void RoundDone()
    {
        var currentPlayer = this.players[this.currentPlayerIndex];
        var previousFinisher = this.players.Select(p => p.FinishedNumber).Max();

        if (currentPlayer.Cards.Any(c => c.Value > 0) == false)
        {
            currentPlayer.FinishedNumber = previousFinisher + 1;
        }

        var notFinishedPlayers = players.Where(p => p.FinishedNumber == 0);
        if (notFinishedPlayers.Count() <= 1)
        {
            this.Finish();
            return;
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
        this.currentPlayerIndex = GetNextPlayerIndex();
    }

    /// <summary>
    /// Applies the effects from cards that affects the next-in-order player instead the one that played the card.
    /// </summary>
    private void ApplyNextPlayerEffects(NextPlayerAction action, int cardCount)
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
            this.BeginStackingDrawCards();
            this.pullCounter += 2 * cardCount;
        }
        else if (action == NextPlayerAction.Draw4)
        {
            this.BeginStackingDrawCards();
            this.pullCounter += 4 * cardCount;
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

    private int GetNextPlayerIndex()
    {
        int index = this.currentPlayerIndex;
        int count = this.players.Count();
        Player player;

        do
        {
            if (this.gameDirectionRightHand)
            {
                index++;
            }
            else
            {
                index--;
            }

            index %= count;

            if (index < 0)
            {
                index = count + index;
            }

            player = this.players[this.currentPlayerIndex];
        } while (player.FinishedNumber != 0 || player.Active != true);

        return index;
    }
}
