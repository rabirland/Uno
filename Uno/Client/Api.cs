using Uno.Shared;

namespace Uno.Client;

public record Api(HttpClient client)
{
    public LobbyApi Lobby { get; } = new LobbyApi(client);

    public readonly record struct LobbyApi(HttpClient client)
    {
        public Task<CreateLobbyResponse> CreateLobbyAsync(CreateLobbyRequest request) =>
            this.client.PostAsApiJsonAsync<CreateLobbyResponse>(URL.Lobby.Create, request);

        public IAsyncEnumerable<ListenLobbyResponse> ListenLobbyAsync(ListenLobbyRequest request) =>
            this.client.PostAsJsonStreamAsync<ListenLobbyResponse>(URL.Lobby.Listen, request);

        public Task<JoinLobbyResponse> JoinLobbyAsync(JoinLobbyRequest request) =>
            this.client.PostAsApiJsonAsync<JoinLobbyResponse>(URL.Lobby.Join, request);

        public Task<SetReadyResponse> SetReadyAsync(SetReadyRequest request) =>
            this.client.PostAsApiJsonAsync<SetReadyResponse>(URL.Lobby.SetReady, request);
    }
}
