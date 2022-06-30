namespace UnoGame;

public class Game
{
    /// <summary>
    /// The list of all players in the game.
    /// </summary>
    private Player[] players;

    /// <summary>
    /// The array index of the current player.
    /// </summary>
    private int currentPlayerIndex;

    public Game(IEnumerable<string> playerNames)
    {
        this.players = playerNames
            .Select(p => new Player(p))
            .ToArray();
    }

    /// <summary>
    /// The player tries to drop a card.
    /// </summary>
    /// <param name="playerName">The name of the player that tries to drop the card.</param>
    /// <param name="index">The array index of the card within the player's hand.</param>
    public void DropCard(string playerName, int index)
    {
        if (!IsCurrentPlayer(playerName))
        {
            throw new Exception("Not the current player to play");
        }


    }

    private bool IsCurrentPlayer(string playerName)
    {
        return players[currentPlayerIndex].Name == playerName;
    }
}
