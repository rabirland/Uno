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
            AppendTokenCookie(token);

            return new CreateLobbyResponse(string.IsNullOrEmpty(token) == false);
        }

        [HttpPost(URL.Lobby.Join)]
        public JoinLobbyResponse Join(JoinLobbyRequest request)
        {
            if (string.IsNullOrEmpty(request.PlayerName))
            {
                return JoinLobbyResponse.Failed;
            }

            if (string.IsNullOrEmpty(request.LobbyName))
            {
                return JoinLobbyResponse.Failed;
            }

            var token = this.lobbyService.AddPlayerToLobby(request.PlayerName, request.LobbyName);
            AppendTokenCookie(token);

            return new JoinLobbyResponse(string.IsNullOrEmpty(token) == false);
        }

        [HttpPost(URL.Lobby.Listen)]
        public async Task Listen(ListenLobbyRequest request)
        {
            var token = GetTokenCookie();

            if (string.IsNullOrEmpty(token))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await Response.CompleteAsync();
            }

            var lobby = lobbyService.FindLobbyByPlayerToken(token);

            if (lobby == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await Response.CompleteAsync();
            }

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

        private void AppendTokenCookie(string token)
        {
            var options = new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                Secure = true,
                HttpOnly = true,
            };
            Response.Cookies.Append(Consts.CookieKeys.PlayerToken, token, options);
        }

        private string GetTokenCookie()
        {
            return Request.Cookies[Consts.CookieKeys.PlayerToken] ?? string.Empty;
        }
    }
}
