using Uno.Server.Annotation;
using Uno.Server.Models.Lobby;

namespace Uno.Server.LobbyService
{
    [Service(AsSingleton = true)]
    public class LobbyService : ILobbyService
    {
        private const int TokenLength = 256;
        private readonly Dictionary<string, Lobby> lobbies = new();

        public event Action<Lobby> OnLobbyChange = _ => { };

        /// <inheritdoc/>
        public string CreateLobby(string name, string adminName)
        {
            if (lobbies.ContainsKey(name))
            {
                return string.Empty;
            }

            var token = TokenCreator.CreateRandomToken(TokenLength);
            var lobby = new Lobby(token);
            if (lobby.AddPlayer(adminName) == false)
            {
                return string.Empty;
            }

            lobbies.Add(name, lobby);
            return lobby.Token;
        }

        /// <inheritdoc/>
        public bool AddPlayerToLobby(string playerName, string lobbyName)
        {
            if (lobbies.TryGetValue(lobbyName, out var lobby))
            {
                lock (lobby.Lock)
                {
                    return lobby.AddPlayer(playerName);
                }
            }
            else
            {
                return false;
            }
        }
    }
}
