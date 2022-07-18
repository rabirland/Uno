using Uno.Server.Models.Lobby;

namespace Uno.Server.LobbyService
{
    public interface ILobbyService
    {
        public event Action<Lobby> OnLobbyChange;

        /// <summary>
        /// Creates a new lobby with the given name.
        /// </summary>
        /// <param name="name">The name of the lobby.</param>
        /// <param name="adminName">The name of the admin player.</param>
        /// <returns>The security token of the admin player.</returns>
        string CreateLobby(string name, string adminName);

        /// <summary>
        /// Attempts to add a player to a lobby.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <returns>The security token of the added player, or empty string if could not be added.</returns>
        string AddPlayerToLobby(string playerName, string lobbyName);

        /// <summary>
        /// Finds the lobby that contains the player with the given token.
        /// </summary>
        /// <param name="token">The player security token.</param>
        /// <returns>The lobby that contains the player or <see langword="null"/> if the player is not found.</returns>
        Lobby? FindLobbyByPlayerToken(string token);
    }
}
