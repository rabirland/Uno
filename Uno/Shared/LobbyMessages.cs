namespace Uno.Shared
{
    public record CreateLobbyRequest(string? LobbyName, string? PlayerName);
    public record CreateLobbyResponse(bool IsSuccessful, string AdminPlayerToken)
    {
        public static CreateLobbyResponse Failed => new CreateLobbyResponse(false, string.Empty);
    }

    public record ListenLobbyRequest();
    public record ListenLobbyResponse(string LobbyName, string PlayerName, IEnumerable<ListenLobbyPlayerEntry> Players)
    {
        public static ListenLobbyResponse Empty { get; } = new ListenLobbyResponse(string.Empty, string.Empty, Array.Empty<ListenLobbyPlayerEntry>());
    }
    public record ListenLobbyPlayerEntry(string Name, bool IsReady);

    public record JoinLobbyRequest(string? PlayerName, string? LobbyName);
    public record JoinLobbyResponse(bool IsSuccessful, string Token)
    {
        public static JoinLobbyResponse Failed => new JoinLobbyResponse(false, string.Empty);
    }

    public record SetReadyRequest(bool IsReady);
    public record SetReadyResponse();
}
