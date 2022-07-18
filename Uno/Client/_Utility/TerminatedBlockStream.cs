using System.Net.WebSockets;
using System.Text;

namespace Uno.Client
{
    public class TerminatedBlockStream : IDisposable
    {
        private readonly char terminator;
        private readonly Stream stream;
        private readonly StreamReader reader;

        public TerminatedBlockStream(Stream stream, char temrinator)
        {
            this.stream = stream;
            this.reader = new StreamReader(stream);
            this.terminator = temrinator;
        }

        public async Task<string> ReadAsync()
        {
            StringBuilder builder = new();
            bool finish = false;

            do
            {
                int read = reader.Read();
                if (read >= 0)
                {
                    char current = (char)read;

                    if (current == terminator)
                    {
                        finish = true;
                    }
                    else
                    {
                        builder.Append(current);
                    }
                }
                else
                {
                    await Task.Delay(50);
                }
            }
            while (finish == false);

            return builder.ToString();
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }
    }
}
