namespace Uno.Client;

/// <summary>
/// Utility used to store and retrieve parameters between page switches.
/// </summary>
public class PageSwitchParameters
{
    private static Dictionary<string, string> queuedParameters = new();

    public static void Set(string key, string parameter)
    {
        queuedParameters.Add(key, parameter);
    }

    public static string Get(string key)
    {
        var parameter = queuedParameters[key];
        queuedParameters.Remove(key);

        return parameter;
    }
}
