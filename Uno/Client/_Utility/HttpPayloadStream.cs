using System.Net.WebSockets;
using System.Text;

namespace Uno.Client
{
    public class HttpPayloadStream
    {
        private readonly StreamReader reader;
        private readonly Stream stream;

        public HttpPayloadStream(Stream stream)
        {
            this.stream = stream;
            this.reader = new StreamReader(stream);
        }

        public async Task<string> ReadAsync()
        {
            var sizeText = await reader.ReadLineAsync();

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
}
