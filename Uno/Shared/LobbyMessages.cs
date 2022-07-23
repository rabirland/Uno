namespace Uno.Shared
{
    public record CreateLobbyRequest(string? LobbyName, string? PlayerName);
    public record CreateLobbyResponse(bool IsSuccessful)
    {
        public static CreateLobbyResponse Failed => new CreateLobbyResponse(false);
    }

    public record ListenLobbyRequest();
    public record ListenLobbyResponse(string LobbyName, IEnumerable<string> Players);

    public record JoinLobbyRequest(string? PlayerName, string? LobbyName);
    public record JoinLobbyResponse(bool IsSuccessful)
    {
        public static JoinLobbyResponse Failed => new JoinLobbyResponse(false);
    }
}
