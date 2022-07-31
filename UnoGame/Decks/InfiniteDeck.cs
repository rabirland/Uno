namespace UnoGame.Decks;

/// <summary>
/// Deck that has infinite amount of cards.
/// </summary>
public class InfiniteDeck : IDeck
{
    /// <inheritdoc/>
    public int RemainingCards => CardMetadata.ValidCards.Count;

    /// <inheritdoc/>
    public CardFace Pull()
    {
        var cardCount = CardMetadata.ValidCards.Count;
        var index = Random.Shared.Next(0, cardCount);

        return CardMetadata.ValidCards[index].Face;
    }

    /// <inheritdoc/>
    public void Push(CardFace card)
    {
    }
}
