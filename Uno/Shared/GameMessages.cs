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
    /// <param name="CurrentPlayerName">The name of the player whose round is ongoing.</param>
    /// <param name="RoundPhase">The phae of the current round.</param>
    public record GameStatus(
        IEnumerable<GameMessages.PlayerHand> OtherPlayerCards,
        IEnumerable<GameMessages.CardCount> Cards,
        int DeckRemainingCards,
        IEnumerable<GameMessages.CardFace> PlayedCards,
        string CurrentPlayerName,
        GameMessages.RoundPhase RoundPhase,
        GameMessages.CardColor ActiveColor);
}

//=========================================================================================== Play Card
public record PlayCardRequest(string GameId, GameMessages.CardFace Card, int Count);
public record PlayCardResponse(bool IsSuccess)
{
    public static PlayCardResponse Empty => new PlayCardResponse(false);
}

//=========================================================================================== Pull Card
public record PullCardRequest(string GameId);
public record PullCardResponse()
{
    public static PullCardResponse Empty => new PullCardResponse();
}

//=========================================================================================== Pick Player
public record PickPlayerRequest(string GameId, string PlayerName);
public record PickPlayerResponse()
{
    public static PickPlayerResponse Empty => new PickPlayerResponse();
}

//=========================================================================================== Pick Color
public record PickColorRequest(string GameId, GameMessages.CardColor Color);
public record PickColorResponse()
{
    public static PickColorResponse Empty => new PickColorResponse();
}

//=========================================================================================== Common Models

public static class GameMessages
{
    public enum RoundPhase
    {
        Card,
        Color,
        Player,
    }

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