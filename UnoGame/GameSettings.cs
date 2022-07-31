namespace UnoGame;

/// <summary>
/// The settings of a game.
/// </summary>
/// <param name="StartCardCount">The amount of cards in the player's hands when the game begins.</param>
/// <param name="StackDraw2">Whether it's allowed to stack +2 cards.</param>
/// <param name="StackDraw4">Whether it's allowed to stack +4 cards.</param>
public record GameSettings(int StartCardCount, bool StackDraw2, bool StackDraw4)
{
    public static GameSettings Default => new GameSettings(7, true, true);

    public int StartCardCount { get; } = StartCardCount;

    public bool StackDraw2 { get; } = StackDraw2;

    public bool StackDraw4 { get; } = StackDraw4;
}
