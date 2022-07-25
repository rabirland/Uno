namespace Uno.Client.GameService;

public class GameService : IGameService
{
    /// <inheritdoc/>
    public string Token { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string PlayerName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string AdminPlayerName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public bool IsAdmin => this.PlayerName == this.AdminPlayerName;

    /// <inheritdoc/>
    public void Set(string token, string playerName, string adminPlayerName)
    {
        this.Token = token;
        this.PlayerName = playerName;
        this.AdminPlayerName = adminPlayerName;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.Token = string.Empty;
        this.PlayerName = string.Empty;
    }
}
