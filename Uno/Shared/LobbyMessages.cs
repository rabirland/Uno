namespace Uno.Shared
{
    public class CreateLobbyRequest
    {
        public string? LobbyName { get; set; }

        public string? PlayerName { get; set; }
    }
    public record CreateLobbyResponse(bool IsSuccessful, string token)
    {
        public static CreateLobbyResponse Failed => new CreateLobbyResponse(false, string.Empty);
    }

    public class ListenLobbyRequest
    {
        public string? Token { get; set; }
    }
    public class ListenLobbyResponse
    {
        public string? Test { get; set; }
    }
}
