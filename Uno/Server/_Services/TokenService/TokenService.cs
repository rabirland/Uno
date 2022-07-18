using Uno.Server.Annotation;

namespace Uno.Server.TokenService
{
    // TODO: Clean up unused tokens
    [Service(AsSingleton = true)]
    public class TokenService : ITokenService
    {
        private const int TokenLength = 256;

        private static readonly string TokenCharacters =
            "abcdefghijklmnopqrstuvwxyz" + 
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "0123456789"
            + "-.$#&@*+%=<>~";

        private readonly Dictionary<string, string> tokens = new();

        /// <inheritdoc/>
        public string GenerateToken(string gameId)
        {
            var token = TokenCreator.CreateRandomToken(TokenLength);
            this.tokens[gameId] = token;

            return token;
        }

        /// <inheritdoc/>
        public string GetToken(string gameId)
        {
            return this.tokens[gameId];
        }
    }
}
