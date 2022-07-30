﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uno.Server.Annotation;
using Uno.Server.GameService;
using Uno.Server.Models.Game;
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
            return JoinGameResponse.Failed;
        }

        var token = GetPlayerToken(game.GameId);
        var isAdmin = game.AdminPlayerToken == token;

        if (isAdmin)
        {
            if (token == null) // Theoretically, should NEVER happen.
            {
                throw new Exception("The admin is set to null token");
            }

            if (game.TryAddPlayer(new Models.Game.GamePlayer(request.PlayerName, token)))
            {
                return new JoinGameResponse(true);
            }
            else
            {
                return JoinGameResponse.Failed;
            }
        }
        else if (token != null)
        {
            // If the player already has a token for this game, the player is trying to join twice. Should use Rejoin endpoint.
            // Except for the admin, who will have a token already assigned, but no name picked yet.

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JoinGameResponse.Failed;
        }
        else
        {
            // Otherwise a new player is joining.
            token = TokenCreator.CreateRandomToken(64);

            if (game.TryAddPlayer(new Models.Game.GamePlayer(request.PlayerName, token)))
            {
                AppendPlayerToken(game.GameId, token);
                return new JoinGameResponse(true);
            }
            else
            {
                return JoinGameResponse.Failed;
            }
        }
    }

    [HttpPost(URL.Game.Rejoin)]
    public RejoinGameResponse Rejoin(RejoinGameRequest request)
    {
        var game = gameService.GetGame(request.GameId);
        
        if (game == null)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return RejoinGameResponse.Failed;
        }

        var token = GetPlayerToken(game.GameId);

        if (token == null)
        {
            return RejoinGameResponse.Failed;
        }

        var playerEntry = game.Players.FirstOrDefault(p => p.Token == token);

        if (playerEntry != null)
        {
            return new RejoinGameResponse(true, playerEntry.PlayerName);
        }
        else
        {
            // When the admin player first joins the game, he already has a token assigned, but still haven't picked a name.
            if (game.AdminPlayerToken == token)
            {
                return RejoinGameResponse.Failed;
            }
            // The player has a token for *this* game, but is not registered for the game.
            // Shouldn't happen under normal conditions.
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return RejoinGameResponse.Failed;
        }
    }

    [DataStream]
    [HttpPost(URL.Game.Listen)]
    public IEnumerable<ListenGameResponse> Listen(ListenGameRequest request)
    {
        if (string.IsNullOrEmpty(request.GameId))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            yield break;
        }

        var token = GetPlayerToken(request.GameId);

        if (string.IsNullOrEmpty(token))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            yield break;
        }

        var gameEntry = gameService.GetGame(request.GameId);
        if (gameEntry == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            yield break;
        }

        // Only allow a player to listen who is joined to the game.
        var listeningPlayer = gameEntry.Players.FirstOrDefault(p => p.Token == token);
        if (listeningPlayer == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            yield break;
        }

        while (true)
        {
            // Bit of an abuse of IEnumerable, but desperate times needs desperate measures. Each function checks if the game is in the
            // correct state and simply returns if not.
            foreach (var status in ReportLobbyStatus(gameEntry, token))
            {
                yield return status;
                Thread.Sleep(250); // Should return when something happens
            }

            foreach (var status in ReportGameStatus(gameEntry, token))
            {
                yield return status;
                Thread.Sleep(250); // Should return when something happens
            }
        }
    }

    [NonAction]
    public void ListenEnded(ListenGameRequest request)
    {
        if (string.IsNullOrEmpty(request.GameId))
        {
            return;
        }

        var token = GetPlayerToken(request.GameId);

        if (string.IsNullOrEmpty(token))
        {
            return;
        }

        var gameEntry = gameService.GetGame(request.GameId);

        if (gameEntry == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        // TODO: Handle multiple listening. Currently if the player opens two pages, and closes one,
        // the player is removed, despite still having an open listening.
        gameEntry.RemovePlayer(token);
    }

    [HttpPost(URL.Game.StartGame)]
    public StartGameResponse StartGame(StartGameRequest request)
    {
        if (string.IsNullOrEmpty(request.GameId))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return StartGameResponse.Failed;
        }

        var token = GetPlayerToken(request.GameId);

        if (string.IsNullOrEmpty(token))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return StartGameResponse.Failed;
        }

        var gameEntry = this.gameService.GetGame(request.GameId);

        if (gameEntry == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return StartGameResponse.Failed;
        }

        // Only the admin can start the game.
        if (gameEntry.AdminPlayerToken != token)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return StartGameResponse.Failed;
        }

        if (gameEntry.CanStart == false)
        {
            return StartGameResponse.Failed;
        }

        gameEntry.StartGame();
        return new StartGameResponse();
    }

    [HttpPost(URL.Game.DropCard)]
    public DropCardResponse DropCard(DropCardRequest request)
    {
        if (string.IsNullOrEmpty(request.GameId))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return DropCardResponse.Empty;
        }

        var token = GetPlayerToken(request.GameId);

        if (string.IsNullOrEmpty(token))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return DropCardResponse.Empty;
        }

        var gameEntry = gameService.GetGame(request.GameId);
        if (gameEntry == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return DropCardResponse.Empty;
        }

        var player = gameEntry.Players.FirstOrDefault(p => p.Token == token);
        if (player == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return DropCardResponse.Empty;
        }

        if (gameEntry.Status != GameStatus.Running)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return DropCardResponse.Empty;
        }

        if (gameEntry.Game == null)
        {
            throw new Exception("Game is running but the Uno Game is not initialized");
        }

        var cardColor = EnumMapper.CardColor.ToUno(request.Card.Color);
        var cardType = EnumMapper.CardType.ToUno(request.Card.Type);

        try
        {
            gameEntry.Game.DropCard(player.Token, new UnoGame.CardFace(cardType, cardColor), request.Count);
            return new DropCardResponse();
        }
        catch
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return DropCardResponse.Empty;
        }
    }

    private IEnumerable<ListenGameResponse> ReportLobbyStatus(GameEntry gameEntry, string token)
    {
        while (gameEntry.Status == GameStatus.InLobby)
        {
            var palyers = gameEntry.Players.Select(p => p.PlayerName);
            var admin = gameEntry.AdminPlayer;
            // Only the admin can see the can start flag
            var canStart = token == gameEntry.AdminPlayerToken
                ? gameEntry.CanStart
                : false;

            yield return ListenGameResponse.AwaitingStart(palyers, admin?.PlayerName ?? string.Empty, canStart);
        }
    }

    private IEnumerable<ListenGameResponse> ReportGameStatus(GameEntry gameEntry, string listenerToken)
    {
        if (gameEntry.Status != GameStatus.Running)
        {
            yield break;
        }

        var game = gameEntry.Game;

        if (game == default)
        {
            throw new Exception("Game is running but UnoGame is not initialized");
        }

        while (gameEntry.Status == GameStatus.Running)
        {
            var player = gameEntry.Players.First(p => p.Token == listenerToken);
            var adminPlayer = gameEntry.Players.First(p => p.Token == gameEntry.AdminPlayerToken);

            var gamePlayer = game.Players.First(p => p.Id == player.Token);

            var otherPlayers = game
                   .Players
                   .Where(p => p.Id != gamePlayer.Id)
                   .Select(p =>
                   {
                       var playerEntry = gameEntry.Players.First(gp => gp.Token == p.Id);
                       var cardCount = p.Cards.Select(c => c.Value).Sum();
                       return new GameMessages.PlayerHand(playerEntry.PlayerName, cardCount);
                   });

            var cardsInHand = gamePlayer
                .Cards
                .Where(c => c.Value > 0)
                .Select(c => new GameMessages.CardCount(
                    EnumMapper.CardColor.ToGameMessageResponse(c.Key.Color),
                    EnumMapper.CardType.ToGameMessageResponse(c.Key.Type),
                    c.Value));

            var deckRemainingCards = game.Deck.RemainingCards;

            var playedCards = game
                .PlayedCards
                .Select(p => new GameMessages.CardFace(
                    EnumMapper.CardColor.ToGameMessageResponse(p.Color),
                    EnumMapper.CardType.ToGameMessageResponse(p.Type)));

            var currentPlayerEntry = gameEntry.Players.First(p => p.Token == game.CurrentPlayerId);

            var gameStatus = new ListenGameResponse.GameStatus(
                otherPlayers,
                cardsInHand,
                deckRemainingCards,
                playedCards,
                currentPlayerEntry.PlayerName);

            yield return new ListenGameResponse(
                adminPlayer.PlayerName,
                null,
                gameStatus);
        }
    }

    private string? GetPlayerToken(string gameId)
    {
        var token = Request.Cookies[$"{Consts.CookieKeys.PlayerToken}_{gameId}"];
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
