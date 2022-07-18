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
        /// <returns>If the lobby is created, the security token of the lobby. Otherwise an empty string.</returns>
        string CreateLobby(string name, string adminName);

        /// <summary>
        /// Attempts to add a player to a lobby.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="lobbyName">The name of the lobby.</param>
        /// <returns><see langword="true"/> if the player could be added.</returns>
        bool AddPlayerToLobby(string playerName, string lobbyName);
    }
}
