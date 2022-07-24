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

            var id = this.CreateId();
            var lobby = new Lobby(id, name, adminName);
            var adminToken = lobby.AddPlayer(adminName);
            if (string.IsNullOrEmpty(adminToken))
            {
                return string.Empty;
            }

            lobbies.Add(lobby);
            return adminToken;
        }

        /// <inheritdoc/>
        public string AddPlayerToLobby(string playerName, long lobbyId)
        {
            var lobby = lobbies.FirstOrDefault(l => l.Id == lobbyId);
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
        public void RemoveLobby(long id)
        {
            var index = this.lobbies.FindIndex(l => l.Id == id);
            this.lobbies.RemoveAt(index);
        }

        private long CreateId()
        {
            long id;
            do
            {
                id = Random.Shared.NextInt64();
            } while (this.lobbies.Any(l => l.Id == id));

            return id;
        }
    }
}
