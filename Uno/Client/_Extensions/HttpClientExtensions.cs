using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Text.Json;
using System.Net.Http.Json;

namespace Uno.Client;

/// <summary>
/// Extension methods for <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a HTTP POST message to the given URL and begins watching the response for continuous stream of payloads.
    /// Whenever a new payload arrives, the async enumerable advances.
    /// </summary>
    /// <typeparam name="TResponse">The type of the expected payload.</typeparam>
    /// <param name="client">The HTTP client to use for the operation.</param>
    /// <param name="url">The URL of the request.</param>
    /// <param name="request">The request object to send for POST.</param>
    /// <returns>An async enumerable.</returns>
    /// <exception cref="Exception">Throws when the received response can not be deserialized as <typeparamref name="TResponse"/>.</exception>
    public static async IAsyncEnumerable<TResponse> PostAsJsonStreamAsync<TResponse>(this HttpClient client, string url, object request)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Content = JsonContent.Create(request, request.GetType());

        // This is the most crucial part, without this the HttpClient will wait until the whole response body has arrived, thus will hang forever for a streamed content.
        httpRequest.SetBrowserResponseStreamingEnabled(true);

        using var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var blockStream = new HttpPayloadStream(stream);

        var responseJson = await blockStream.ReadAsync();

        while (string.IsNullOrEmpty(responseJson) == false)
        {
            var responseObj = JsonSerializer.Deserialize<TResponse>(responseJson);

            if (responseObj == null)
            {
                throw new Exception("Invalid response json");
            }

            yield return responseObj;

            responseJson = await blockStream.ReadAsync();
        }
    }

    public static async Task<TResponse> PostAsApiJsonAsync<TResponse>(this HttpClient client, string url, object request)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Content = JsonContent.Create(request, request.GetType());

        var response = await client.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var tst = new StreamReader(await response.Content.ReadAsStreamAsync());
        var fasz = await tst.ReadToEndAsync();

        var ret = await response.Content.ReadFromJsonAsync<TResponse>();

        if (ret == null)
        {
            throw new Exception("Invalid JSON");
        }

        return ret;
    }
}
