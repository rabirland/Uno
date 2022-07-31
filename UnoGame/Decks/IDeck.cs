namespace UnoGame.Decks;

public interface IDeck
{
    /// <summary>
    /// The amount of cards left in the deck.
    /// </summary>
    public int RemainingCards { get; }

    /// <summary>
    /// Pulls a card from the top of the deck.
    /// </summary>
    /// <returns>The pulled card.</returns>
    public CardFace Pull();

    /// <summary>
    /// Pushes a card to the bottom of the deck.
    /// </summary>
    /// <param name="card">The card to push.</param>
    public void Push(CardFace card);
}
