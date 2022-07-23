namespace Uno.Server
{
    /// <summary>
    /// Utility to create random string tokens.
    /// </summary>
    public static class TokenCreator
    {
        private static readonly string TokenCharacters =
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "0123456789"
            + "-.$#&@*+%=<>~";

        /// <summary>
        /// Generates a random string token with the given length.
        /// </summary>
        /// <param name="length">The desired length of the token.</param>
        /// <returns>The generated token.</returns>
        public static string CreateRandomToken(int length)
        {
            Span<char> token = stackalloc char[length];

            for (int i = 0; i < token.Length; i++)
            {
                char randomCh = TokenCharacters[Random.Shared.Next(0, TokenCharacters.Length)];
                token[i] = randomCh;
            }

            return new string(token);
        }
    }
}
