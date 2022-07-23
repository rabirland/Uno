namespace Uno.Server.Models.Game;

public record GameEntry(
    string GameName,
    UnoGame.Game Game,
    IEnumerable<GamePlayer> Players,
    string AdminPlayerName);
