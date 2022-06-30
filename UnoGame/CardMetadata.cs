namespace UnoGame;

/// <summary>
/// Holds static informations about cards.
/// </summary>
public static class CardMetadata
{
    public static readonly IReadOnlyList<Card> ValidCards = new[]
    {
        /////////////////////////////////////////////// RED
        new Card(CardType.Num0, CardColor.Red),
        new Card(CardType.Num1, CardColor.Red),
        new Card(CardType.Num2, CardColor.Red),
        new Card(CardType.Num3, CardColor.Red),
        new Card(CardType.Num4, CardColor.Red),
        new Card(CardType.Num5, CardColor.Red),
        new Card(CardType.Num6, CardColor.Red),
        new Card(CardType.Num7, CardColor.Red),
        new Card(CardType.Num8, CardColor.Red),
        new Card(CardType.Num9, CardColor.Red),
        new Card(CardType.Block, CardColor.Red, NextAction: NextPlayerActions.Skip),
        new Card(CardType.Reverse, CardColor.Red, ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(CardType.Plus2, CardColor.Red, NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Green
        new Card(CardType.Num0, CardColor.Green),
        new Card(CardType.Num1, CardColor.Green),
        new Card(CardType.Num2, CardColor.Green),
        new Card(CardType.Num3, CardColor.Green),
        new Card(CardType.Num4, CardColor.Green),
        new Card(CardType.Num5, CardColor.Green),
        new Card(CardType.Num6, CardColor.Green),
        new Card(CardType.Num7, CardColor.Green),
        new Card(CardType.Num8, CardColor.Green),
        new Card(CardType.Num9, CardColor.Green),
        new Card(CardType.Block, CardColor.Green, NextAction: NextPlayerActions.Skip),
        new Card(CardType.Reverse, CardColor.Green, ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(CardType.Plus2, CardColor.Green, NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Blue
        new Card(CardType.Num0, CardColor.Blue),
        new Card(CardType.Num1, CardColor.Blue),
        new Card(CardType.Num2, CardColor.Blue),
        new Card(CardType.Num3, CardColor.Blue),
        new Card(CardType.Num4, CardColor.Blue),
        new Card(CardType.Num5, CardColor.Blue),
        new Card(CardType.Num6, CardColor.Blue),
        new Card(CardType.Num7, CardColor.Blue),
        new Card(CardType.Num8, CardColor.Blue),
        new Card(CardType.Num9, CardColor.Blue),
        new Card(CardType.Block, CardColor.Blue, NextAction: NextPlayerActions.Skip),
        new Card(CardType.Reverse, CardColor.Blue, ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(CardType.Plus2, CardColor.Blue, NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Yellow
        new Card(CardType.Num0, CardColor.Yellow),
        new Card(CardType.Num1, CardColor.Yellow),
        new Card(CardType.Num2, CardColor.Yellow),
        new Card(CardType.Num3, CardColor.Yellow),
        new Card(CardType.Num4, CardColor.Yellow),
        new Card(CardType.Num5, CardColor.Yellow),
        new Card(CardType.Num6, CardColor.Yellow),
        new Card(CardType.Num7, CardColor.Yellow),
        new Card(CardType.Num8, CardColor.Yellow),
        new Card(CardType.Num9, CardColor.Yellow),
        new Card(CardType.Block, CardColor.Yellow, NextAction: NextPlayerActions.Skip),
        new Card(CardType.Reverse, CardColor.Yellow, ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(CardType.Plus2, CardColor.Yellow, NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Colorless
        new Card(CardType.Plus4, CardColor.Colorless, CurrentPlayerAction.PickColor, NextPlayerActions.Draw4),
        new Card(CardType.ColorChange, CardColor.Colorless, CurrentPlayerAction.PickColor),
        new Card(CardType.Swap, CardColor.Colorless, CurrentPlayerAction.PickPlayer),
    };
}
