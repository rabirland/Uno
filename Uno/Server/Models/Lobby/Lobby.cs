namespace Uno.Server.Models.Lobby
{
    /// <summary>
    /// Represents a lobby for players to wait for the beginning of a game.
    /// </summary>
    public class Lobby
    {
        private readonly List<LobbyPlayer> players = new();

        public Lobby(string securityToken)
        {
            this.Token = securityToken;
        }

        /// <summary>
        /// The object used for thread locking operations on one lobby.
        /// </summary>
        public object Lock { get; } = new();

        public string Token { get; }

        /// <summary>
        /// Adds a player to the lobby.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <returns><see langword="true"/> if the player could be added, otherwise <see langword="false"/>.</returns>
        public bool AddPlayer(string playerName)
        {
            if (players.Any(p => p.Name == playerName))
            {
                return false;
            }
            else
            {
                players.Add(new LobbyPlayer(playerName));
                return true;
            }
        }
    }
}
