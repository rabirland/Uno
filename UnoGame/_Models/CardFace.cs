namespace UnoGame;

public readonly record struct CardFace(
    CardType Type,
    CardColor Color)
{
    public CardType Type { get; } = Type;
    public CardColor Color { get; } = Color;

    public override string ToString()
    {
        return $"{Color}/{Type}";
    }
}
