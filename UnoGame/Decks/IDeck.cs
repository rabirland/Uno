namespace UnoGame.Decks;

public interface IDeck
{
    /// <summary>
    /// The cards left in the deck.
    /// </summary>
    public int RemainingCards { get; }

    /// <summary>
    /// Pulls a card from the deck.
    /// </summary>
    /// <returns>The pulled card.</returns>
    public CardFace Pull();
}
