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

    [HttpPost(URL.Game.Create)]
    public CreateGameResponse Create(CreateGameRequest request)
    {
        var result = this.gameService.Create();
        AppendPlayerToken(result.GameId, result.AdminToken);

        return new CreateGameResponse(true, result.GameId);
    }

    [HttpPost(URL.Game.Join)]
    public JoinGameResponse Join(JoinGameRequest request)
    {
        var game = this.gameService.GetGame(request.GameId);

        if (game == null)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return new JoinGameResponse(false);
        }

        var token = GetPlayerToken(game.GameId);
        if (token == null)
        {
            token = TokenCreator.CreateRandomToken(64);
        }

        if (game.Players.Any(p => p.Token == token))
        {
            return new JoinGameResponse(true); // Also give player name
        }

        if (game.TryAddPlayer(new Models.Game.GamePlayer(request.PlayerName, token)))
        {
            AppendPlayerToken(game.GameId, token);
            return new JoinGameResponse(true);
        }
        else
        {
            return new JoinGameResponse(false);
        }
    }

    [DataStream]
    [HttpPost(URL.Game.Listen)]
    public IEnumerable<ListenGameResponse> Listen(ListenGameRequest request)
    {
        var token = ControllerExtensions.GetPlayerToken(this);

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

    private string? GetPlayerToken(string gameId)
    {
        var token = Request.Cookies[Consts.CookieKeys.PlayerToken];
        return token;
    }

    private void AppendPlayerToken(string gameId, string token)
    {
        var options = new CookieOptions();
        options.HttpOnly = true;
        options.SameSite = SameSiteMode.Strict;
        options.Secure = true;
        var key = $"{Consts.CookieKeys.PlayerToken}_{gameId}";
        Response.Cookies.Append(key, token, options);
    }
}
