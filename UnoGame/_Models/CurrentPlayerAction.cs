namespace UnoGame;

/// <summary>
/// Actions that happens with the player who dropped the card.
/// </summary>
public enum CurrentPlayerAction
{
    /// <summary>
    /// No action.
    /// </summary>
    None,

    /// <summary>
    /// Waiting for the player to pick a color.
    /// </summary>
    PickColor,

    /// <summary>
    /// Waiting for the player to pick a player.
    /// </summary>
    PickPlayer,
}
