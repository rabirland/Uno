using Uno.Server.Models.Game;

namespace Uno.Server.GameService;

public interface IGameService
{
    /// <summary>
    /// Creates a new game with the given player list.
    /// </summary>
    /// <param name="gameName">The name of the game.</param>
    /// <param name="players">The list of players.</param>
    /// <param name="adminPlayerName">The name of the admin player.</param>
    void Create(
        string gameName,
        IEnumerable<GamePlayer> players,
        string adminPlayerName);

    /// <summary>
    /// Searches a game entry by a player's token.
    /// </summary>
    /// <param name="token">The player token.</param>
    /// <returns>The game entry, or <see langword="default"/> if no player is found.</returns>
    GameEntry? FindGameByPlayerToken(string token);
}
