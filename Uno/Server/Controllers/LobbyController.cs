using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;
using Uno.Server.LobbyService;
using Uno.Shared;

namespace Uno.Server.Controllers
{
    [ApiController]
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

        [HttpPost(URL.Lobby.Create)]
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

        [HttpPost(URL.Lobby.Listen)]
        public async Task Listen(ListenLobbyRequest request)
        {
            Response.StatusCode = (int)HttpStatusCode.PartialContent;
            Response.ContentType = "application/json";
            await Response.StartAsync();

            try
            {
                var isConnectionEnded = false;
                do
                {
                    await Task.Delay(500);
                    var json = JsonSerializer.Serialize(new ListenLobbyResponse { Test = $"Wot" });

                    await Response.WriteAsync($"{json.Length}\r\n", Encoding.UTF8);
                    await Response.WriteAsync(json, Encoding.UTF8);

                    isConnectionEnded = HttpContext.RequestAborted.IsCancellationRequested;
                }
                while (isConnectionEnded == false);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
