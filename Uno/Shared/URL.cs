namespace Uno.Shared
{
    /// <summary>
    /// Contains the URLs of the API.
    /// </summary>
    public static class URL
    {
        public static class Game
        {
            public const string Controller = "/api/game";
            public const string Create = Controller + "/create";
            public const string Join = Controller + "/join";
            public const string Rejoin = Controller + "/rejoin";
            public const string Listen = Controller + "/listen";
            public const string StartGame = Controller + "/start-game";
            public const string DropCard = Controller + "/drop-card";
        }
    }
}
