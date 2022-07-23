namespace Uno.Server.Models.Lobby
{
    /// <summary>
    /// Represents a lobby for players to wait for the beginning of a game.
    /// </summary>
    public class Lobby
    {
        /// <summary>
        /// The object used for thread locking operations on one lobby.
        /// </summary>
        private readonly object lockObj = new();

        private readonly List<LobbyPlayer> players = new();

        public Lobby(string name, string adminName)
        {
            this.Name = name;
            this.AdminPlayerName = adminName;
        }

        public string Name { get; }

        public string AdminPlayerName { get; }

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
            lock (this.lockObj)
            {
                if (players.Any(p => p.Name == playerName))
                {
                    return string.Empty;
                }
                else
                {
                    var token = TokenCreator.CreateRandomToken(256);
                    players.Add(new LobbyPlayer(playerName, token, false));
                    return token;
                }
            }
        }

        /// <summary>
        /// Removes a player from the lobby given it's token.
        /// </summary>
        /// <param name="token">The player's security token.</param>
        public void RemovePlayerByToken(string token)
        {
            lock (this.lockObj)
            {
                var index = this.players.FindIndex(p => p.Token == token);

                if (index >= 0)
                {
                    this.players.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Checks whether any player in the lobby has the given security token.
        /// </summary>
        /// <param name="playerToken">The token to search for.</param>
        /// <returns><see langword="true"/> if any player in this lobby has the given security token.</returns>
        public bool HasPlayerByToken(string playerToken)
        {
            lock (this.lockObj)
            {
                return players.Any(p => p.Token == playerToken);
            }
        }

        /// <summary>
        /// Sets a player's ready state given the player token.
        /// </summary>
        /// <param name="token">The token of the player.</param>
        /// <param name="isReady">Whether the player is ready.</param>
        public void SetPlayerReadyByToken(string token, bool isReady)
        {
            lock (lockObj)
            {
                var index = players.FindIndex(p => p.Token == token);
                this.players[index] = this.players[index] with { IsReady = isReady };
            }
        }

        /// <summary>
        /// Searches the player with the given token.
        /// </summary>
        /// <param name="token">The player token.</param>
        /// <returns>The player, or <see langword="default"/> if there is no player with the given token.</returns>
        public LobbyPlayer? GetPlayerByToken(string token)
        {
            return players.FirstOrDefault(p => p.Token == token);
        }
    }
}
