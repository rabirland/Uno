using Uno.Server.Annotation;
using Uno.Server.Models.Lobby;

namespace Uno.Server.LobbyService
{
    [Service(AsSingleton = true)]
    public class LobbyService : ILobbyService
    {
        private const int TokenLength = 256;
        private readonly List<Lobby> lobbies = new();

        public event Action<Lobby> OnLobbyChange = _ => { };

        /// <inheritdoc/>
        public string CreateLobby(string name, string adminName)
        {
            var existingLobby = lobbies.Any(l => l.Name == name);
            if (existingLobby)
            {
                return string.Empty;
            }

            var lobby = new Lobby(name, adminName);
            var adminToken = lobby.AddPlayer(adminName);
            if (string.IsNullOrEmpty(adminToken))
            {
                return string.Empty;
            }

            lobbies.Add(lobby);
            return adminToken;
        }

        /// <inheritdoc/>
        public string AddPlayerToLobby(string playerName, string lobbyName)
        {
            var lobby = lobbies.FirstOrDefault(l => l.Name == lobbyName);
            if (lobby != null)
            {
                var token = lobby.AddPlayer(playerName);
                return token;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public Lobby? FindLobbyByPlayerToken(string token)
        {
            return lobbies.FirstOrDefault(l => l.HasPlayerByToken(token));
        }

        /// <inheritdoc/>
        public void RemoveLobby(string lobbyName)
        {
            var index = this.lobbies.FindIndex(l => l.Name == lobbyName);
            this.lobbies.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Lobby? GetLobby(string lobbyName)
        {
            return lobbies.FirstOrDefault(l => l.Name == lobbyName);
        }
    }
}
