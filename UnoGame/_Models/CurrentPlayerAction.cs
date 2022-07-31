namespace UnoGame;

/// <summary>
/// Actions that happens with the player who dropped the card.
/// </summary>
/// <remarks>
/// The value of the enums indicates the order the operations should be done during game.
/// </remarks>
[Flags]
public enum CurrentPlayerAction
{
    /// <summary>
    /// The card has no effect on the player who dropped it.
    /// </summary>
    None = 0,

    /// <summary>
    /// The player have to pick a player to swap the cards in their hands with.
    /// </summary>
    SwapHandDeckWithPlayer = (1 << 0),

    /// <summary>
    /// The player have to pick an active color.
    /// </summary>
    PickColor = (1 << 1),
}
