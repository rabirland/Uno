namespace UnoGame;

/// <summary>
/// The phase of the current round.
/// </summary>
public enum RoundPhase
{
    /// <summary>
    /// The player must play or pull a card.
    /// </summary>
    Card,

    /// <summary>
    /// The player must pick a color.
    /// </summary>
    Color,

    /// <summary>
    /// The player must pick a player.
    /// </summary>
    Player,
}
