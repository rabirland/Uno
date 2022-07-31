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

        public Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request) =>
           this.client.PostAsApiJsonAsync<JoinGameResponse>(URL.Game.Join, request);

        public Task<RejoinGameResponse> RejoinGameAsync(RejoinGameRequest request) =>
           this.client.PostAsApiJsonAsync<RejoinGameResponse>(URL.Game.Rejoin, request);

        public Task<StartGameResponse> StartGameAsync(StartGameRequest request) =>
           this.client.PostAsApiJsonAsync<StartGameResponse>(URL.Game.StartGame, request);

        public Task<PlayCardResponse> PlayCardAsync(PlayCardRequest request) =>
           this.client.PostAsApiJsonAsync<PlayCardResponse>(URL.Game.PlayCard, request);

        public Task<PullCardResponse> PullCardAsync(PullCardRequest request) =>
           this.client.PostAsApiJsonAsync<PullCardResponse>(URL.Game.PullCard, request);

        public Task<PickPlayerResponse> PickPlayerAsync(PickPlayerRequest request) =>
           this.client.PostAsApiJsonAsync<PickPlayerResponse>(URL.Game.PickPlayer, request);

        public Task<PickColorResponse> PickColorAsync(PickColorRequest request) =>
           this.client.PostAsApiJsonAsync<PickColorResponse>(URL.Game.PickColor, request);
    }
}
