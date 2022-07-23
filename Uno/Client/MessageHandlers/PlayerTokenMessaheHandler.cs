using Uno.Client.GameService;
using Uno.Shared;

namespace Uno.Client.MessageHandlers
{
    /// <summary>
    /// Http message handler that appends the user tokens to the request headers, if there is any token.
    /// </summary>
    public class PlayerTokenMessageHandler : DelegatingHandler
    {
        private readonly Uri allowedBaseAddress;
        private readonly IGameService gameService;

        public PlayerTokenMessageHandler(Uri allowedBaseAddress, IGameService gameService)
            : base()
        {
            this.allowedBaseAddress = allowedBaseAddress;
            this.gameService = gameService;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return this.SendAsync(request, cancellationToken).Result;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TryAppendTokens(request, cancellationToken);

            return base.SendAsync(request, cancellationToken);
        }

        private void TryAppendTokens(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri;

            if (uri == null)
            {
                return;
            }

            var isSelfApiAccess = this.allowedBaseAddress.IsBaseOf(uri);

            if (isSelfApiAccess == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.gameService.Token) == false)
            {
                request.Headers.Add(SharedConsts.HttpHeaders.PlayerToken, this.gameService.Token);
            }
        }
    }
}