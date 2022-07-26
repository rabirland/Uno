using Uno.Shared;

namespace Uno.Client;

public record Api(HttpClient client)
{
    public GameApi Game { get; } = new GameApi(client);

    public readonly record struct GameApi(HttpClient client)
    {
        public IAsyncEnumerable<ListenGameResponse> ListenGameAsync(ListenGameRequest request) =>
            this.client.PostAsJsonStreamAsync<ListenGameResponse>(URL.Game.Listen, request);

        public Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request) =>
            this.client.PostAsApiJsonAsync<CreateGameResponse>(URL.Game.Create, request);
    }
}
