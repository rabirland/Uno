namespace UnoGame;

/// <summary>
/// Holds static informations about cards.
/// </summary>
public static class CardMetadata
{
    public static readonly IReadOnlyList<Card> ValidCards = new[]
    {
        /////////////////////////////////////////////// RED
        new Card(new CardFace(CardType.Num0, CardColor.Red)),
        new Card(new CardFace(CardType.Num1, CardColor.Red)),
        new Card(new CardFace(CardType.Num2, CardColor.Red)),
        new Card(new CardFace(CardType.Num3, CardColor.Red)),
        new Card(new CardFace(CardType.Num4, CardColor.Red)),
        new Card(new CardFace(CardType.Num5, CardColor.Red)),
        new Card(new CardFace(CardType.Num6, CardColor.Red)),
        new Card(new CardFace(CardType.Num7, CardColor.Red)),
        new Card(new CardFace(CardType.Num8, CardColor.Red)),
        new Card(new CardFace(CardType.Num9, CardColor.Red)),
        new Card(new CardFace(CardType.Block, CardColor.Red), NextAction: NextPlayerActions.Skip),
        new Card(new CardFace(CardType.Reverse, CardColor.Red), ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(new CardFace(CardType.Plus2, CardColor.Red), NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Green
        new Card(new CardFace(CardType.Num0, CardColor.Green)),
        new Card(new CardFace(CardType.Num1, CardColor.Green)),
        new Card(new CardFace(CardType.Num2, CardColor.Green)),
        new Card(new CardFace(CardType.Num3, CardColor.Green)),
        new Card(new CardFace(CardType.Num4, CardColor.Green)),
        new Card(new CardFace(CardType.Num5, CardColor.Green)),
        new Card(new CardFace(CardType.Num6, CardColor.Green)),
        new Card(new CardFace(CardType.Num7, CardColor.Green)),
        new Card(new CardFace(CardType.Num8, CardColor.Green)),
        new Card(new CardFace(CardType.Num9, CardColor.Green)),
        new Card(new CardFace(CardType.Block, CardColor.Green), NextAction: NextPlayerActions.Skip),
        new Card(new CardFace(CardType.Reverse, CardColor.Green), ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(new CardFace(CardType.Plus2, CardColor.Green), NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Blue
        new Card(new CardFace(CardType.Num0, CardColor.Blue)),
        new Card(new CardFace(CardType.Num1, CardColor.Blue)),
        new Card(new CardFace(CardType.Num2, CardColor.Blue)),
        new Card(new CardFace(CardType.Num3, CardColor.Blue)),
        new Card(new CardFace(CardType.Num4, CardColor.Blue)),
        new Card(new CardFace(CardType.Num5, CardColor.Blue)),
        new Card(new CardFace(CardType.Num6, CardColor.Blue)),
        new Card(new CardFace(CardType.Num7, CardColor.Blue)),
        new Card(new CardFace(CardType.Num8, CardColor.Blue)),
        new Card(new CardFace(CardType.Num9, CardColor.Blue)),
        new Card(new CardFace(CardType.Block, CardColor.Blue), NextAction: NextPlayerActions.Skip),
        new Card(new CardFace(CardType.Reverse, CardColor.Blue), ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(new CardFace(CardType.Plus2, CardColor.Blue), NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Yellow
        new Card(new CardFace(CardType.Num0, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num1, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num2, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num3, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num4, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num5, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num6, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num7, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num8, CardColor.Yellow)),
        new Card(new CardFace(CardType.Num9, CardColor.Yellow)),
        new Card(new CardFace(CardType.Block, CardColor.Yellow), NextAction: NextPlayerActions.Skip),
        new Card(new CardFace(CardType.Reverse, CardColor.Yellow), ImmediateAction: ImmediateAction.ReverseOrder),
        new Card(new CardFace(CardType.Plus2, CardColor.Yellow), NextAction: NextPlayerActions.Draw2),

        /////////////////////////////////////////////// Colorless
        new Card(new CardFace(CardType.Plus4, CardColor.Colorless), CurrentPlayerAction.PickColor, NextPlayerActions.Draw4),
        new Card(new CardFace(CardType.ColorChange, CardColor.Colorless), CurrentPlayerAction.PickColor),
        new Card(new CardFace(CardType.Swap, CardColor.Colorless), CurrentPlayerAction.PickPlayer),
    };
}
