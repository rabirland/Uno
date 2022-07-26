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
    /// Joins a player to a game.
    /// </summary>
    /// <param name="gameId">The ID of the game.</param>
    /// <param name="player">The player.</param>
    /// <returns><see langword="true"/> if the player is added to the game.</returns>
    bool JoinPlayerToGame(string gameId, GamePlayer player);
}
