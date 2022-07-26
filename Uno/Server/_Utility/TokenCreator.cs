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
            "0123456789";

        private static readonly string SpecialCharacters = "-.$#&@*+%=<>~";

        /// <summary>
        /// Generates a random string token with the given length.
        /// </summary>
        /// <param name="length">The desired length of the token.</param>
        /// <param name="asSimple">Whether to allow special characters.</param>
        /// <returns>The generated token.</returns>
        public static string CreateRandomToken(int length, bool asSimple = false)
        {
            Span<char> token = stackalloc char[length];

            var characterSet = TokenCharacters;

            if (asSimple == false)
            {
                characterSet += SpecialCharacters;
            }

            for (int i = 0; i < token.Length; i++)
            {
                char randomCh = characterSet[Random.Shared.Next(0, characterSet.Length)];
                token[i] = randomCh;
            }

            return new string(token);
        }
    }
}
