using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uno.Server.Annotation;
using Uno.Server.GameService;
using Uno.Server.Models.Game;
using Uno.Shared;

namespace Uno.Server.Controllers;

[ApiController]
public class UnoGameController : Controller
{
    private Dictionary<string, int> connectionCounter = new Dictionary<string, int>();

    private readonly IGameService gameService;

    public UnoGameController(IGameService gameService)
    {
        this.gameService = gameService;
    }

    [HttpPost(URL.Game.Uno.Create)]
    public CreateGameResponse Create(CreateGameRequest request)
    {
        var result = this.gameService.Create();
        AppendPlayerToken(result.GameId, result.AdminToken);

        return new CreateGameResponse(true, result.GameId);
    }

    [HttpPost(URL.Game.Uno.Join)]
    public JoinGameResponse Join(JoinGameRequest request)
    {
        var game = this.gameService.GetGame(request.GameId);

        if (string.IsNullOrEmpty(request.PlayerName))
        {
            return JoinGameResponse.Failed;
        }

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

            if (game.TryAddPlayer(new GamePlayer(request.PlayerName, token)))
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

            if (game.TryAddPlayer(new GamePlayer(request.PlayerName, token)))
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

    [HttpPost(URL.Game.Uno.Rejoin)]
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
    [HttpPost(URL.Game.Uno.Listen)]
    public IEnumerable<ListenGameResponse> Listen(ListenGameRequest request)
    {
        if (string.IsNullOrEmpty(request.GameId))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
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
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            yield break;
        }

        // Only allow a player to listen who is joined to the game.
        var listeningPlayer = gameEntry.Players.FirstOrDefault(p => p.Token == token);
        if (listeningPlayer == default)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            yield break;
        }

        gameEntry.SetPlayerActiveByToken(token);
        this.IncrementConnections(token);

        while (true)
        {
            // Bit of an abuse of IEnumerable, but desperate times needs desperate measures. Each function checks if the game is in the
            // correct state and simply returns if not.
            foreach (var status in ReportLobbyStatus(gameEntry, token))
            {
                yield return status;
                Thread.Sleep(1000 / 30); // 30Hz, should report only when something change
            }

            foreach (var status in ReportGameRunningStatus(gameEntry, token))
            {
                yield return status;
                Thread.Sleep(1000 / 30); // 30 Hz
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

        var connections = this.DecrementConnections(token);
        if (connections <= 0)
        {
            gameEntry.SetPlayerInactiveByToken(token);
        }
    }

    [HttpPost(URL.Game.Uno.StartGame)]
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

    [HttpPost(URL.Game.Uno.PlayCard)]
    public PlayCardResponse PlayCard(PlayCardRequest request)
    {
        if (AuthenticatePlayerForActiveGame(request.GameId, out var playerData) == false)
        {
            return PlayCardResponse.Empty;
        }

        var cardColor = EnumMapper.CardColor.ToUno(request.Card.Color);
        var cardType = EnumMapper.CardType.ToUno(request.Card.Type);

        try
        {
            var success = playerData.Game.PlayCard(new UnoGame.CardFace(cardType, cardColor), request.Count);
            return new PlayCardResponse(success);
        }
        catch
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PlayCardResponse.Empty;
        }
    }

    [HttpPost(URL.Game.Uno.PullCard)]
    public PullCardResponse PullCard(PullCardRequest request)
    {
        if (AuthenticatePlayerForActiveGame(request.GameId, out var playerData) == false)
        {
            return PullCardResponse.Empty;
        }

        try
        {
            playerData.Game.PullCard();
            return new PullCardResponse();
        }
        catch
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PullCardResponse.Empty;
        }
    }

    [HttpPost(URL.Game.Uno.PickPlayer)]
    public PickPlayerResponse PickPlayer(PickPlayerRequest request)
    {
        if (AuthenticatePlayerForActiveGame(request.GameId, out var playerData) == false)
        {
            return PickPlayerResponse.Empty;
        }

        try
        {
            var player = playerData.GameEntry.Players.First(p => p.PlayerName == request.PlayerName);
            playerData.Game.PickPlayer(player.Token);
            return new PickPlayerResponse();
        }
        catch
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PickPlayerResponse.Empty;
        }
    }

    [HttpPost(URL.Game.Uno.PickColor)]
    public PickColorResponse PickColor(PickColorRequest request)
    {
        if (AuthenticatePlayerForActiveGame(request.GameId, out var playerData) == false)
        {
            return PickColorResponse.Empty;
        }

        try
        {
            var color = EnumMapper.CardColor.ToUno(request.Color);
            playerData.Game.PickColor(color);
            return new PickColorResponse();
        }
        catch
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PickColorResponse.Empty;
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

            yield return ListenGameResponse.AwaitingStart(palyers, admin?.PlayerName ?? string.Empty, canStart, gameEntry.PreviousGameLeaderboard);
        }
    }

    private IEnumerable<ListenGameResponse> ReportGameRunningStatus(GameEntry gameEntry, string listenerToken)
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

            var currentPlayer = game.Players.First(p => p.Id == player.Token);

            var otherPlayers = game
                   .Players
                   .Where(p => p.Id != currentPlayer.Id)
                   .Select(p =>
                   {
                       var playerEntry = gameEntry.Players.First(gp => gp.Token == p.Id);
                       var cardCount = p.Cards.Select(c => c.Value).Sum();
                       return new GameMessages.PlayerHand(playerEntry.PlayerName, cardCount, p.FinishedNumber);
                   });

            var cardsInHand = currentPlayer
                .Cards
                .Where(c => c.Value > 0)
                .Select(c => new GameMessages.CardCount(
                    EnumMapper.CardColor.ToGameMessageResponse(c.Key.Color),
                    EnumMapper.CardType.ToGameMessageResponse(c.Key.Type),
                    c.Value));

            var deckRemainingCardCount = game.Deck.RemainingCards;

            var playedCards = game
                .PlayedCards
                .Select(p => new GameMessages.CardFace(
                    EnumMapper.CardColor.ToGameMessageResponse(p.Color),
                    EnumMapper.CardType.ToGameMessageResponse(p.Type)))
                .TakeLast(15);

            var currentPlayerEntry = gameEntry.Players.First(p => p.Token.Equals(game.CurrentPlayerId, StringComparison.InvariantCulture));

            var roundPhase = EnumMapper.RoundPhase.ToGameMessageResponse(game.RoundPhase);

            var activeColor = EnumMapper.CardColor.ToGameMessageResponse(game.ActiveColor);

            var gameStatus = new ListenGameResponse.GameStatus(
                OtherPlayerCards: otherPlayers,
                Cards: cardsInHand,
                DeckRemainingCardCount: deckRemainingCardCount,
                PlayedCards: playedCards,
                CurrentPlayerName: currentPlayerEntry.PlayerName,
                CurrentPlayerFinishedNumber: currentPlayer.FinishedNumber,
                IsGameFinished: false,
                RoundPhase: roundPhase,
                ActiveColor: activeColor,
                DrawStackCount: game.DrawCardCounter,
                Clockwise: game.DirectionClockwise);

            yield return new ListenGameResponse(
                adminPlayer.PlayerName,
                null,
                gameStatus,
                null);
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

    private bool AuthenticatePlayerForActiveGame(string gameId, out (GameEntry GameEntry, UnoGame.Game Game, GamePlayer Player) data)
    {
        data = default;

        if (string.IsNullOrEmpty(gameId))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return false;
        }

        var token = GetPlayerToken(gameId);

        if (string.IsNullOrEmpty(token))
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return false;
        }

        var gameEntry = gameService.GetGame(gameId);
        if (gameEntry == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return false;
        }

        var player = gameEntry.Players.FirstOrDefault(p => p.Token == token);
        if (player == default)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return false;
        }

        if (gameEntry.Status != GameStatus.Running)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return false;
        }

        if (gameEntry.Game == null)
        {
            throw new Exception("Game is running but the Uno Game is not initialized");
        }

        if (player.Token != gameEntry.Game.CurrentPlayerId)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return false;
        }

        data = (gameEntry, gameEntry.Game, player);
        return true;
    }

    private void IncrementConnections(string token)
    {
        var count = 0;
        if (this.connectionCounter.TryGetValue(token, out count))
        {
        }

        this.connectionCounter[token] = count + 1;
    }

    private int DecrementConnections(string token)
    {
        var count = 0;
        if (this.connectionCounter.TryGetValue(token, out count))
        {
        }

        count -= 1;

        if (count > 0)
        {
            this.connectionCounter[token] = count;
        }
        else
        {
            this.connectionCounter.Remove(token);
        }

        return Math.Max(count, 0);
    }
}
