namespace Uno.Server.TokenService
{
    /// <summary>
    /// Stores the JWT tokens for each game Id.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Creates and stores the security token for a game id.
        /// </summary>
        /// <param name="gameId">The id of the game.</param>
        /// <returns>The generated JWT token.</returns>
        string GenerateToken(string gameId);

        /// <summary>
        /// Gets the security token of a given game id.
        /// </summary>
        /// <param name="gameId">The id of the game.</param>
        /// <returns>The token of the game.</returns>
        string GetToken(string gameId);
    }
}
