namespace UnoGame;

/// <summary>
/// Actions that happens with the player next in order.
/// </summary>
public enum NextPlayerActions
{
    /// <summary>
    /// No action.
    /// </summary>
    None,

    /// <summary>
    /// The player's round is skip.
    /// </summary>
    Skip,

    /// <summary>
    /// The player draws 2 cards.
    /// </summary>
    Draw2,

    /// <summary>
    /// The player draws 4 cards.
    /// </summary>
    Draw4,
}
