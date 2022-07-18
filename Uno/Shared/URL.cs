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
        }
    }
}
