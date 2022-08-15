using Uno.Shared;

namespace Uno.Client;

public record Api(HttpClient client)
{
    public UnoGameApi UnoGame { get; } = new UnoGameApi(client);

    public readonly record struct UnoGameApi(HttpClient client)
    {
        public IAsyncEnumerable<ListenGameResponse> ListenGameAsync(ListenGameRequest request) =>
            this.client.PostAsJsonStreamAsync<ListenGameResponse>(URL.Game.Uno.Listen, request);

        public Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request) =>
            this.client.PostAsApiJsonAsync<CreateGameResponse>(URL.Game.Uno.Create, request);

        public Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request) =>
           this.client.PostAsApiJsonAsync<JoinGameResponse>(URL.Game.Uno.Join, request);

        public Task<RejoinGameResponse> RejoinGameAsync(RejoinGameRequest request) =>
           this.client.PostAsApiJsonAsync<RejoinGameResponse>(URL.Game.Uno.Rejoin, request);

        public Task<StartGameResponse> StartGameAsync(StartGameRequest request) =>
           this.client.PostAsApiJsonAsync<StartGameResponse>(URL.Game.Uno.StartGame, request);

        public Task<PlayCardResponse> PlayCardAsync(PlayCardRequest request) =>
           this.client.PostAsApiJsonAsync<PlayCardResponse>(URL.Game.Uno.PlayCard, request);

        public Task<PullCardResponse> PullCardAsync(PullCardRequest request) =>
           this.client.PostAsApiJsonAsync<PullCardResponse>(URL.Game.Uno.PullCard, request);

        public Task<PickPlayerResponse> PickPlayerAsync(PickPlayerRequest request) =>
           this.client.PostAsApiJsonAsync<PickPlayerResponse>(URL.Game.Uno.PickPlayer, request);

        public Task<PickColorResponse> PickColorAsync(PickColorRequest request) =>
           this.client.PostAsApiJsonAsync<PickColorResponse>(URL.Game.Uno.PickColor, request);
    }
}
