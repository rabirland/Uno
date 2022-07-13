namespace UnoGame;

public readonly record struct Card(
    CardFace Face,
    CurrentPlayerAction CurrentAction = CurrentPlayerAction.None,
    NextPlayerActions NextAction = NextPlayerActions.None,
    ImmediateAction ImmediateAction = ImmediateAction.None)
{
    public CardFace Face { get; } = Face;
    public CurrentPlayerAction CurrentAction { get; } = CurrentAction;
    public NextPlayerActions NextAction { get; } = NextAction;

    public override string ToString()
    {
        return Face.ToString();
    }
}
