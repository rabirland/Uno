using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uno.Server.Annotation;
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

        [DataStream]
        [HttpPost(URL.Lobby.Listen)]
        public IEnumerable<ListenLobbyResponse> Listen(ListenLobbyRequest request)
        {
            var token = GetTokenCookie();

            if (string.IsNullOrEmpty(token))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                yield break;
            }

            var lobby = lobbyService.FindLobbyByPlayerToken(token);

            if (lobby == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                yield break;
            }

            while (true)
            {
                Thread.Sleep(1000);
                yield return new ListenLobbyResponse(
                    lobby.Name,
                    lobby.Players.Select(p => p.Name));
            }
        }

        [NonAction]
        public void ListenEnded()
        {
            var token = GetTokenCookie();

            if (string.IsNullOrEmpty(token))
            {
                return; // Shouldn't happen
            }

            var lobby = lobbyService.FindLobbyByPlayerToken(token);

            if (lobby == null)
            {
                return; // Shouldn't happen
            }

            lobby.RemovePlayerByToken(token);
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
