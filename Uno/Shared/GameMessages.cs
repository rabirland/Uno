namespace Uno.Shared;

//=========================================================================================== Listen Game
public record ListenGameRequest();
public record ListenGameResponse(
    ListenGameResponse.GameStatus State,
    string CurrentPlayerName,
    IEnumerable<ListenGameResponse.PlayerEntry> OtherPlayers,
    IEnumerable<ListenGameResponse.CardCount> Cards)
{
    public enum GameStatus
    {
        /// <summary>
        /// The game is waiting for every player to connect.
        /// </summary>
        AwaitingConnecting,

        /// <summary>
        /// The game is running.
        /// </summary>
        Running,
    }

    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow,

        Colorless,
    }

    public enum CardType
    {
        Num0,
        Num1,
        Num2,
        Num3,
        Num4,
        Num5,
        Num6,
        Num7,
        Num8,
        Num9,
        Block,
        Reverse,
        Swap,
        Plus2,
        Plus4,
        ColorChange,
    }

    public record PlayerEntry(string PlayerName, int CardCount);

    public record CardCount(CardColor Color, CardType Type, int Count);
}