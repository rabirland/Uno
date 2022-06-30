namespace UnoGame;

public readonly record struct Card(
    CardType Type,
    CardColor Color,
    CurrentPlayerAction CurrentAction = CurrentPlayerAction.None,
    NextPlayerActions NextAction = NextPlayerActions.None,
    ImmediateAction ImmediateAction = ImmediateAction.None)
{
    public CardType Type { get; } = Type;
    public CardColor Color { get; } = Color;
    public CurrentPlayerAction CurrentAction { get; } = CurrentAction;
    public NextPlayerActions NextAction { get; } = NextAction;

    /// <summary>
    /// Checks if the two cards are the same by their type and color only.
    /// </summary>
    /// <param name="type">The card type.</param>
    /// <param name="color">The card color.</param>
    /// <returns><see langword="true"/> if the two cards are the same.</returns>
    public bool Is(CardType type, CardColor color)
    {
        return this.Type == type && this.Color == color;
    }

    /// <summary>
    /// Checks if the two cards are the same by their type and color only.
    /// </summary>
    /// <param name="card">The other card to compare with.</param>
    /// <returns><see langword="true"/> if the two cards are the same.</returns>
    public bool Is(Card card)
    {
        return this.Is(card.Type, card.Color);
    }

    public override string ToString()
    {
        return $"{Color}/{Type}";
    }
}
