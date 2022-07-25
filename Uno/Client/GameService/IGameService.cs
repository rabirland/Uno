namespace Uno.Client.GameService;

/// <summary>
/// Stores the player data for a game.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// The security token of the player.
    /// </summary>
    string Token { get; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    string PlayerName { get; }

    /// <summary>
    /// The name of the admin player.
    /// </summary>
    string AdminPlayerName { get; }

    /// <summary>
    /// Whether this player is the admin.
    /// </summary>
    bool IsAdmin { get; }

    /// <summary>
    /// Sets the player data.
    /// </summary>
    /// <param name="token">The security token.</param>
    /// <param name="playerName">The player name.</param>
    /// <param name="adminPlayerName">The name of the admin player.</param>
    void Set(string token, string playerName, string adminPlayerName);

    /// <summary>
    /// Clears the player data.
    /// </summary>
    void Clear();
}
