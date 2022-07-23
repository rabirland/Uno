namespace Uno.Client.GameService;

public class GameService : IGameService
{
    /// <inheritdoc/>
    public string Token { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string PlayerName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public void Set(string token, string playerName)
    {
        this.Token = token;
        this.PlayerName = playerName;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.Token = string.Empty;
        this.PlayerName = string.Empty;
    }
}
