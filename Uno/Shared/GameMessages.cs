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

//=========================================================================================== Rejoin Game
public record RejoinGameRequest(string GameId);
public record RejoinGameResponse(bool IsSuccess, string PlayerName)
{
    public static RejoinGameResponse Failed => new RejoinGameResponse(false, string.Empty);
}

//=========================================================================================== Start Game
public record StartGameRequest(string GameId);
public record StartGameResponse()
{
    public static StartGameResponse Failed => new StartGameResponse();
}

//=========================================================================================== Listen Game
public record ListenGameRequest(string GameId);
public record ListenGameResponse(
    string AdminPlayerName,
    ListenGameResponse.LobbyStatus? Lobby,
    ListenGameResponse.GameStatus? Game)
{
    public static ListenGameResponse Empty => new ListenGameResponse(
        string.Empty,
        null,
        null);

    public static ListenGameResponse AwaitingStart(IEnumerable<string> players, string adminName, bool canStart) => new ListenGameResponse(
        adminName,
        new LobbyStatus(players, canStart),
        null);

    /// <summary>
    /// Stores the status of the game lobby.
    /// </summary>
    /// <param name="Players">The name of all joined players.</param>
    /// <param name="CanStart">Whether the game can be started.</param>
    public record LobbyStatus(IEnumerable<string> Players, bool CanStart);

    /// <summary>
    /// Stores the status of a running game.
    /// </summary>
    /// <param name="OtherPlayerCards">The amount of cards in each player's hands.</param>
    /// <param name="Cards">The cards in the player's hands who is receiving the message.</param>
    /// <param name="DeckRemainingCards">The amount of cards remaining in the deck.</param>
    /// <param name="PlayedCards">The list of cards that was played by the players previously.</param>
    public record GameStatus(
        IEnumerable<PlayerHand> OtherPlayerCards,
        IEnumerable<CardCount> Cards,
        int DeckRemainingCards,
        IEnumerable<CardFace> PlayedCards);

    /// <summary>
    /// The color of an UNO card.
    /// </summary>
    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow,

        Colorless,
    }

    /// <summary>
    /// The type of an UNO card.
    /// </summary>
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

    public record CardFace(CardColor Color, CardType Type);

    public record PlayerHand(string PlayerName, int CardCount);

    public record CardCount(CardColor Color, CardType Type, int Count);
}