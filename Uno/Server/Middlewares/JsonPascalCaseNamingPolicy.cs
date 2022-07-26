using System.Text.Json;

namespace Uno.Server.Middlewares;

public class JsonPascalCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return char.ToUpperInvariant(name[0]) + name[1..];
    }
}
