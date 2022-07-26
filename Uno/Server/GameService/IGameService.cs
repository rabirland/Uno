using Uno.Server.Models.Game;

namespace Uno.Server.GameService;

public interface IGameService
{
    // TODO: Clean up empty / idle games

    /// <summary>
    /// Creates a new game with the given player list.
    /// </summary>
    /// <returns>The result of creating a game.</returns>
    GameCreateResult Create();

    /// <summary>
    /// Searches a game entry by a player's token.
    /// </summary>
    /// <param name="token">The player token.</param>
    /// <returns>The game entry, or <see langword="default"/> if no player is found.</returns>
    GameEntry? FindGameByPlayerToken(string token);

    /// <summary>
    /// Returns a game by it's id.
    /// </summary>
    /// <param name="gameId">The Id of the game.</param>
    /// <returns>The game, or <see langword="null"/> if the game not exists.</returns>
    GameEntry? GetGame(string gameId);
}
