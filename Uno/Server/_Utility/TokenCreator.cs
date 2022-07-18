namespace Uno.Server
{
    public static class TokenCreator
    {
        private static readonly string TokenCharacters =
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "0123456789"
            + "-.$#&@*+%=<>~";

        public static string CreateRandomToken(int length)
        {
            Span<char> token = stackalloc char[length];

            for (int i = 0; i < token.Length; i++)
            {
                char randomCh = TokenCharacters[Random.Shared.Next(0, TokenCharacters.Length)];
            }

            return new string(token);
        }
    }
}
