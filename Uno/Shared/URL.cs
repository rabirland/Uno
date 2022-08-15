namespace Uno.Shared
{
    /// <summary>
    /// Contains the URLs of the API.
    /// </summary>
    public static class URL
    {
        public static class Game
        {
            public static class Uno
            {
                public const string Controller = "/api/game/uno";
                public const string Create = Controller + "/create";
                public const string Join = Controller + "/join";
                public const string Rejoin = Controller + "/rejoin";
                public const string Listen = Controller + "/listen";
                public const string StartGame = Controller + "/start-game";
                public const string PlayCard = Controller + "/play-card";
                public const string PullCard = Controller + "/pull-card";
                public const string PickPlayer = Controller + "/pick-player";
                public const string PickColor = Controller + "/pick-color";
            }
        }
    }
}
