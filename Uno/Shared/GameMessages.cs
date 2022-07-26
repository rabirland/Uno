namespace Uno.Shared;

//=========================================================================================== Create Game
public record CreateGameRequest();
public record CreateGameResponse(bool IsSuccess, string GameId);

//=========================================================================================== Join Game
public record JoinGameRequest(string GameId, string PlayerName);
public record JoinGameResponse(bool IsSuccess)
{
    public static JoinGameResponse Failed => new JoinGameResponse(false);
}

//=========================================================================================== Join Game
public record RejoinGameRequest(string GameId);
public record RejoinGameResponse(bool IsSuccess, string PlayerName)
{
    public static RejoinGameResponse Failed => new RejoinGameResponse(false, string.Empty);
}

//=========================================================================================== Listen Game
public record ListenGameRequest(string GameId);
public record ListenGameResponse(
    ListenGameResponse.GameStatus State,
    string AdminPlayerName,
    string CurrentPlayerName,
    IEnumerable<ListenGameResponse.PlayerEntry> OtherPlayers,
    IEnumerable<ListenGameResponse.CardCount> Cards)
{
    public static ListenGameResponse Empty => new ListenGameResponse(
        GameStatus.Unknown,
        string.Empty,
        string.Empty,
        Enumerable.Empty<PlayerEntry>(),
        Enumerable.Empty<CardCount>());

    public static ListenGameResponse AwaitingStart(IEnumerable<string> players, string adminName) => new ListenGameResponse(
        GameStatus.AwaitingStart,
        adminName,
        string.Empty,
        players.Select(p => new PlayerEntry(p, 0)),
        Enumerable.Empty<CardCount>());

    public enum GameStatus
    {
		/// <summary>
		/// Unused default value.
		/// </summary>
        Unknown,

        /// <summary>
        /// The game is waiting for every player to connect.
        /// </summary>
        AwaitingStart,

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