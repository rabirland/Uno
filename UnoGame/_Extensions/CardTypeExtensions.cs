namespace UnoGame;

/// <summary>
/// Extension methods for <see cref="CardType"/>.
/// </summary>
public static class CardTypeExtensions
{
    public static bool IsNumerical(this CardType type)
    {
        var value = (int)type;
        var min = (int)CardType.Num0;
        var max = (int)CardType.Num9;

        return value >= min && value <= max;
    }

    public static bool IsDraw(this CardType type)
    {
        return type == CardType.Plus2
            || type == CardType.Plus4;
    }
}
