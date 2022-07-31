namespace UnoGame;

/// <summary>
/// Extension methods for <see cref="CardColor"/>.
/// </summary>
public static class CardColorExtensions
{
    /// <summary>
    /// Checks if the card color is not colorless.
    /// </summary>
    /// <param name="color">The card color to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="color"/> is chromatic.</returns>
    public static bool IsChromatic(this CardColor color)
    {
        return color == CardColor.Blue
            || color == CardColor.Yellow
            || color == CardColor.Red
            || color == CardColor.Green;
    }
}
