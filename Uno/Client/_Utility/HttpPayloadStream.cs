namespace Uno.Client;

/// <summary>
/// Reads separate payloads from a continuous (streamed) HTTP response stream.
/// The protocol of the payloads is: <br/>
/// [amount of characters in payload] [new line] [HTTP payload] ... <br/>
/// E.g.:
/// <code>
/// 17
/// { "Key": "Data" }
/// </code>
/// </summary>
public class HttpPayloadStream : IDisposable
{
    private readonly StreamReader reader;
    private readonly Stream stream;

    public HttpPayloadStream(Stream stream)
    {
        this.stream = stream;
        this.reader = new StreamReader(stream);
    }

    public void Dispose()
    {
        this.reader.Dispose();
    }

    /// <summary>
    /// Reads the next block of the stream.
    /// </summary>
    /// <returns>The read block, or <see langword="null"/> if the connection is closed.</returns>
    /// <exception cref="Exception">Throws when the format of the stream is incorrect.</exception>
    public async Task<string> ReadAsync()
    {
        var sizeText = await reader.ReadLineAsync();

        // ReadLine returns null when the stream is closed.
        if (sizeText == null)
        {
            return string.Empty;
        }

        if (int.TryParse(sizeText, out var payloadSize) == false)
        {
            throw new Exception("Corrupted data stream");
        }

        var payloadBuffer = new char[payloadSize];
        await reader.ReadAsync(payloadBuffer, 0, payloadSize);

        var payload = new string(payloadBuffer);
        return payload;
    }
}