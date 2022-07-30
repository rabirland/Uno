﻿namespace UnoGame;

public class Player
{
    private Dictionary<CardFace, int> cardsInHand;

    public Player(string id)
    {
        this.Id = id;
        this.cardsInHand = CardMetadata
            .ValidCards
            .ToDictionary(
                c => c.Face, // The key is the card face.
                c => 0); // The value is the count in the player's hand.
    }

    /// <summary>
    /// The id of the player.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The current cards in the player's hands.
    /// </summary>
    public IEnumerable<KeyValuePair<CardFace, int>> Cards => this.cardsInHand;

    /// <summary>
    /// Adds a card to the player.
    /// </summary>
    /// <param name="cardFace">The face card to add.</param>
    public void AddCard(CardFace cardFace)
    {
        try
        {
            var currentCount = this.cardsInHand[cardFace];
            this.cardsInHand[cardFace] = currentCount + 1;
        }
        catch
        {
            throw new Exception($"The {Id} player does not knows {cardFace} card.");
        }
    }

    /// <summary>
    /// Removes a card from the player's hand.
    /// </summary>
    /// <param name="cardFace">The face of the card to remove.</param>
    /// <param name="count">The amount of cards to drop.</param>
    /// <returns><see langword="true"/> if the cards were dropped, exactly the amount of <paramref name="count"/>.</returns>
    public bool RemoveCard(CardFace cardFace, int count)
    {
        try
        {
            var currentCount = this.cardsInHand[cardFace];
            if (currentCount >= count)
            {
                this.cardsInHand[cardFace] = currentCount - count;
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            throw new Exception($"The player {Id} has no {cardFace} card in hand.");
        }
    }
}
