﻿using Microsoft.AspNetCore.Mvc;
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

            return new CreateLobbyResponse(string.IsNullOrEmpty(token) == false, token);
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

            return new JoinLobbyResponse(string.IsNullOrEmpty(token) == false, token);
        }

        [DataStream]
        [HttpPost(URL.Lobby.Listen)]
        public IEnumerable<ListenLobbyResponse> Listen(ListenLobbyRequest request)
        {
            var token = GetPlayerToken();

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
                yield return new ListenLobbyResponse(
                    lobby.Name,
                    lobby.Players.Select(p => new ListenLobbyPlayerEntry(p.Name, p.IsReady)));

                Thread.Sleep(100); // Wait for anything to change instead
            }
        }

        [HttpPost(URL.Lobby.SetReady)]
        public SetReadyResponse SetReady(SetReadyRequest request)
        {
            var token = GetPlayerToken();
            var lobby = this.lobbyService.FindLobbyByPlayerToken(token);

            if (lobby == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new SetReadyResponse();
            }

            lobby.SetPlayerReadyByToken(token, request.IsReady);

            return new SetReadyResponse();
        }

        [HttpPost(URL.Lobby.StartGame)]
        public StartGameResponse StartGame()
        {
            var token = GetPlayerToken();

            if (string.IsNullOrEmpty(token))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return StartGameResponse.Empty;
            }

            var lobby = this.lobbyService.FindLobbyByPlayerToken(token);

            if (lobby == default)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return StartGameResponse.Empty;
            }

            var player = lobby.GetPlayerByToken(token);

            if (player == default)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return StartGameResponse.Empty;
            }

            if (player.Name != lobby.AdminPlayerName)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return StartGameResponse.Empty;
            }

            if (lobby.Players.Count() < 2)
            {
                return new StartGameResponse(false, StartGameFailedReason.NotEnoughPlayers);
            }

            var allPlayerReady = lobby.Players.All(p => p.IsReady);

            if (allPlayerReady)
            {
                // TODO: Success
            }
            else
            {
                return new StartGameResponse(false, StartGameFailedReason.PlayerNotReady);
            }
        }

        [NonAction]
        public void ListenEnded()
        {
            var token = GetPlayerToken();

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

        private string GetPlayerToken()
        {
            return Request.Headers[SharedConsts.HttpHeaders.PlayerToken];
        }
    }
}
