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

        while(true)
        {
            yield return new ListenGameResponse(ListenGameStatus.AwaitingConnecting);
        }
    }
}
