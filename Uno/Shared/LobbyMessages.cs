namespace Uno.Shared;

//=========================================================================================== Create Lobby
public record CreateLobbyRequest(string? LobbyName, string? PlayerName);
public record CreateLobbyResponse(bool IsSuccessful, string AdminPlayerToken)
{
    public static CreateLobbyResponse Failed => new CreateLobbyResponse(false, string.Empty);
}

//=========================================================================================== Listen Lobby
public record ListenLobbyRequest();
public record ListenLobbyResponse(
    string LobbyName,
    IEnumerable<ListenLobbyPlayerEntry> Players,
    bool GameReady,
    bool CanStart)
{
    public static ListenLobbyResponse Empty { get; } = new ListenLobbyResponse(
        string.Empty,
        Enumerable.Empty<ListenLobbyPlayerEntry>(),
        false,
        false);
}
public record ListenLobbyPlayerEntry(string Name, bool IsReady);

//=========================================================================================== Join Lobby
public record JoinLobbyRequest(string? PlayerName, string lobbyName);
public record JoinLobbyResponse(bool IsSuccessful, string Token, string AdminPlayerName)
{
    public static JoinLobbyResponse Failed => new JoinLobbyResponse(false, string.Empty, string.Empty);
}

//=========================================================================================== Set Ready
public record SetReadyRequest(bool IsReady);
public record SetReadyResponse();

//=========================================================================================== StartGame
public record StartGameRequest();
public record StartGameResponse(bool IsSuccessful, StartGameFailedReason FailedReason)
{
    public static StartGameResponse Empty => new StartGameResponse(false, StartGameFailedReason.Unknown);
}
public enum StartGameFailedReason
{
    Unknown,
    PlayerNotReady,
    NotEnoughPlayers,

    /// <summary>
    /// The lobby is valid for a game to be started.
    /// </summary>
    Valid,
}

