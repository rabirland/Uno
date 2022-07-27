namespace UnoGame;

/// <summary>
/// The settings of a game.
/// </summary>
/// <param name="StartCardCount">The amount of cards in the player's hands when the game begins.</param>
public record GameSettings(int StartCardCount)
{
    public static GameSettings Default => new GameSettings(7);

    public int StartCardCount { get; } = StartCardCount;
}
