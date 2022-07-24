using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uno.Server.Annotation;
using Uno.Server.GameService;
using Uno.Shared;

namespace Uno.Server.Controllers;

[ApiController]
public class GameController : Controller
{
    private readonly IGameService gameService;

    public GameController(IGameService gameService)
    {
        this.gameService = gameService;
    }

    [DataStream]
    [HttpPost(URL.Game.Listen)]
    public IEnumerable<ListenGameResponse> Listen(ListenGameRequest request)
    {
        var token = this.GetPlayerToken();

        if (token == default)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            yield break;
        }

        var gameEntry = gameService.FindGameByPlayerToken(token);

        if (gameEntry == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            yield break;
        }

        var game = gameEntry.Game;
        var listeningPlayer = gameEntry.Players.First(p => p.Token == token);
        var gamePlayer = game.Players.First(p => p.Name == listeningPlayer.PlayerName);

        while (true)
        {
            var players = game
                .Players
                .Where(p => p.Name != listeningPlayer.PlayerName)
                .Select(p => new ListenGameResponse.PlayerEntry(p.Name, p.Cards.Count()));

            var cards = gamePlayer
                .Cards
                .Select(c => new ListenGameResponse.CardCount(
                    EnumMapper.CardColor.ToListenGameResponse(c.Key.Color),
                    EnumMapper.CardType.ToListenGameResponse(c.Key.Type),
                    c.Value));

            yield return new ListenGameResponse(
                ListenGameResponse.GameStatus.AwaitingConnecting,
                game.CurrentPlayer,
                players,
                cards);
        }
    }
}
