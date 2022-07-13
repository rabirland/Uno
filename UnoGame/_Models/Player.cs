namespace UnoGame;

public class Player
{
    private Dictionary<CardFace, int> cardsInHand;

    public Player(string name)
    {
        this.Name = name;
        this.cardsInHand = CardMetadata
            .ValidCards
            .ToDictionary(
                c => c.Face, // The key is the card face.
                c => 0); // The value is the count in the player's hand.
    }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name { get; }

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
            throw new Exception($"The {Name} player does not knows {cardFace} card.");
        }
    }

    /// <summary>
    /// Removes a card from the player's hand.
    /// </summary>
    /// <param name="cardFace">The face of the card to remove.</param>
    public void RemoveCard(CardFace cardFace)
    {
        try
        {
            var currentCount = this.cardsInHand[cardFace];
            if (currentCount > 0)
            {
                this.cardsInHand[cardFace] = currentCount - 1;
            }
        }
        catch
        {
            throw new Exception($"The player {Name} has no {cardFace} card in hand.");
        }
    }
}
