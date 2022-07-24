using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uno.Server.Annotation;
using Uno.Server.GameService;
using Uno.Server.LobbyService;
using Uno.Server.Models.Game;
using Uno.Server.Models.Lobby;
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
		private readonly IGameService gameService;

		public LobbyController(ILobbyService lobbyService, IGameService gameService)
		{
			this.lobbyService = lobbyService;
			this.gameService = gameService;
		}

		// TODO: handle duplicated lobby names
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

			var token = this.lobbyService.AddPlayerToLobby(request.PlayerName, request.lobbyId);

			return new JoinLobbyResponse(string.IsNullOrEmpty(token) == false, token);
		}

		[DataStream]
		[HttpPost(URL.Lobby.Listen)]
		public IEnumerable<ListenLobbyResponse> Listen(ListenLobbyRequest request)
		{
			var token = this.GetPlayerToken();

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
					lobby.Players.Select(p => new ListenLobbyPlayerEntry(p.Name, p.IsReady)),
					lobby.IsFinished);

				if (lobby.IsFinished)
                {
					yield break;
                }

				Thread.Sleep(100); // TODO: Wait for anything to change instead
			}
		}

		[HttpPost(URL.Lobby.SetReady)]
		public SetReadyResponse SetReady(SetReadyRequest request)
		{
			var token = this.GetPlayerToken();
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
			var token = this.GetPlayerToken();

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

			var startStatus = CanGameBeStarted(lobby);

			if (startStatus == StartGameFailedReason.Valid)
            {
				this.gameService.Create(
					lobby.Name,
					lobby.Players.Select(p => new GamePlayer(p.Name, p.Token)),
					lobby.AdminPlayerName);

				lobby.MarkFinished();
				this.lobbyService.RemoveLobby(lobby.Id);

				return new StartGameResponse(true, startStatus);
			}
			else
            {
				return new StartGameResponse(false, startStatus);
			}
		}

		[NonAction]
		public void ListenEnded()
		{
			var token = this.GetPlayerToken();

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

			// Clean up empty lobbies
			if (lobby.Players.Count() == 0)
			{
				this.lobbyService.RemoveLobby(lobby.Id);
			}
		}

		private StartGameFailedReason CanGameBeStarted(Lobby lobby)
        {
			if (lobby.Players.Count() < 2)
			{
				return StartGameFailedReason.NotEnoughPlayers;
			}

			var allPlayerReady = lobby.Players.All(p => p.IsReady);

			if (allPlayerReady)
			{
				return StartGameFailedReason.Valid;
			}
			else
			{
				return StartGameFailedReason.PlayerNotReady;
			}
		}
	}
}
