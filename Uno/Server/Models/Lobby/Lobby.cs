namespace Uno.Server.Models.Lobby
{
    /// <summary>
    /// Represents a lobby for players to wait for the beginning of a game.
    /// </summary>
    public class Lobby
    {
        private readonly List<LobbyPlayer> players = new();

        public Lobby(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// The object used for thread locking operations on one lobby.
        /// </summary>
        public object Lock { get; } = new();

        /// <summary>
        /// The players within the lobby.
        /// </summary>
        public IEnumerable<LobbyPlayer> Players => this.players;

        /// <summary>
        /// Adds a player to the lobby.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>The security token of the player, or empty string if the player could not be added.</returns>
        public string AddPlayer(string playerName)
        {
            if (players.Any(p => p.Name == playerName))
            {
                return string.Empty;
            }
            else
            {
                var token = TokenCreator.CreateRandomToken(256);
                players.Add(new LobbyPlayer(playerName, token));
                return token;
            }
        }

        /// <summary>
        /// Removes a player from the lobby given it's token.
        /// </summary>
        /// <param name="token">The player's security token.</param>
        public void RemovePlayerByToken(string token)
        {
            var index = this.players.FindIndex(p => p.Token == token);

            if (index >= 0)
            {
                this.players.RemoveAt(index);
            }
        }

        /// <summary>
        /// Checks whether any player in the lobby has the given security token.
        /// </summary>
        /// <param name="playerToken">The token to search for.</param>
        /// <returns><see langword="true"/> if any player in this lobby has the given security token.</returns>
        public bool HasPlayerByToken(string playerToken)
        {
            return players.Any(p => p.Token == playerToken);
        }
    }
}
