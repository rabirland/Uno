namespace Uno.Shared
{
    /// <summary>
    /// Contains the URLs of the API.
    /// </summary>
    public static class URL
    {
        public static class Lobby
        {
            public const string Controller = "/api/lobby";
            public const string Create = Controller + "/create";
            public const string Listen = Controller + "/listen";
            public const string Join = Controller + "/join";
            public const string SetReady = Controller + "/set-ready";
            public const string StartGame = Controller + "/start-game";
        }

        public static class Game
        {
            public const string Controller = "/api/game";
            public const string Listen = Controller + "/listen";
        }
    }
}
