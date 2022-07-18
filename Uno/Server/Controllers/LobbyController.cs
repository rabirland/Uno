using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Uno.Server.LobbyService;
using Uno.Shared;

namespace Uno.Server.Controllers
{
    public class LobbyController : Controller
    {
        /// <summary>
        /// Fired when any of the lobbies change.
        /// </summary>
        private readonly ILobbyService lobbyService;

        public LobbyController(ILobbyService lobbyService)
        {
            this.lobbyService = lobbyService;
        }

        [HttpPost(URL.Lobby.Crate)]
        public CreateLobbyResponse Create(CreateLobbyRequest request)
        {
            if (string.IsNullOrEmpty(request.LobbyName))
            {
                return CreateLobbyResponse.Failed;
            }

            if (string.IsNullOrEmpty(request.PlayerName))
            {
                return CreateLobbyResponse.Failed;
            }

            var token = this.lobbyService.CreateLobby(request.LobbyName, request.PlayerName);

            return new CreateLobbyResponse(string.IsNullOrEmpty(token) == false, token);
        }

        [HttpGet(URL.Lobby.Listen)]
        public async Task Listen()
        {
            Response.StatusCode = (int)HttpStatusCode.PartialContent;
            Response.ContentType = "application/json";
            await Response.StartAsync();

            int index = 0;
            try
            {
                while (index < 3)
                {
                    await Task.Delay(1000);
                    var json = JsonSerializer.Serialize(new ListenLobbyResponse { Test = "Wot" });
                    await Response.WriteAsync(json + "¤"); // Block terminator character
                    index++;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
