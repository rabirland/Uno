namespace UnoGame;

public readonly record struct Card(
    CardFace Face,
    CurrentPlayerAction CurrentAction = CurrentPlayerAction.None,
    NextPlayerAction NextAction = NextPlayerAction.None,
    ImmediateAction ImmediateAction = ImmediateAction.None)
{
    public CardFace Face { get; } = Face;
    public CurrentPlayerAction CurrentAction { get; } = CurrentAction;
    public NextPlayerAction NextPlayerAction { get; } = NextAction;

    public override string ToString()
    {
        return Face.ToString();
    }
}
