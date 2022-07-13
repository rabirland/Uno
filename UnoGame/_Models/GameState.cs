namespace UnoGame
{
    /// <summary>
    /// The current state of the game.
    /// </summary>
    /// <param name="CurrentPlayer">The name of the current player.</param>
    public record GameState(
        string CurrentPlayer,
        IEnumerable<PlayerState> Players);

    /// <summary>
    /// The current state of a single player.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Cards"></param>
    public readonly record struct PlayerState(
        string Name,
        IEnumerable<PlayerCardCount> Cards);

    /// <summary>
    /// The amount of a single card in a player's hand.
    /// </summary>
    /// <param name="Face">The face of the card.</param>
    /// <param name="Count">The count in the player's hand.</param>
    public readonly record struct PlayerCardCount(CardFace Face, int Count);
}
