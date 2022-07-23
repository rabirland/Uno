namespace Uno.Shared;

//=========================================================================================== Create Lobby
public record CreateLobbyRequest(string? LobbyName, string? PlayerName);
public record CreateLobbyResponse(bool IsSuccessful, string AdminPlayerToken)
{
    public static CreateLobbyResponse Failed => new CreateLobbyResponse(false, string.Empty);
}

//=========================================================================================== Listen Lobby
public record ListenLobbyRequest();
public record ListenLobbyResponse(string LobbyName, IEnumerable<ListenLobbyPlayerEntry> Players)
{
    public static ListenLobbyResponse Empty { get; } = new ListenLobbyResponse(string.Empty, Array.Empty<ListenLobbyPlayerEntry>());
}
public record ListenLobbyPlayerEntry(string Name, bool IsReady);

//=========================================================================================== Join Lobby
public record JoinLobbyRequest(string? PlayerName, string? LobbyName);
public record JoinLobbyResponse(bool IsSuccessful, string Token)
{
    public static JoinLobbyResponse Failed => new JoinLobbyResponse(false, string.Empty);
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
}

