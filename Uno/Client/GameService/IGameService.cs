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
    /// Sets the player data.
    /// </summary>
    /// <param name="token">The security token.</param>
    /// <param name="playerName">The player name.</param>
    void Set(string token, string playerName);

    /// <summary>
    /// Clears the player data.
    /// </summary>
    void Clear();
}
