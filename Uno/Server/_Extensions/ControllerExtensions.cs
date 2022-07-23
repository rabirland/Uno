using Microsoft.AspNetCore.Mvc;
using Uno.Shared;

namespace Uno.Server;

/// <summary>
/// Extension methods for <see cref="Controller"/>.
/// </summary>
public static class ControllerExtensions
{
    public static string GetPlayerToken(this Controller controller)
    {
        return controller.Request.Headers[SharedConsts.HttpHeaders.PlayerToken];
    }
}
