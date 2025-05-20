using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Text;

namespace RadiantConnect.Utilities
{
    internal static class SocketUtil
    {

        internal static string GetUnixTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        internal static bool IsValidGuid([StringSyntax(StringSyntaxAttribute.GuidFormat)] string guid) => Guid.TryParse(guid, out Guid _) && guid.Contains('-');

        internal static async Task AsyncSocketWrite(SslStream sslStream, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await sslStream.WriteAsync(buffer, 0, buffer.Length);
            await sslStream.FlushAsync();
        }

        internal static async Task<string> AsyncSocketRead(SslStream sslStream)
        {
            int byteCount;
            byte[] bytes = new byte[1024];
            StringBuilder contentBuilder = new();

            do
            {
                try { byteCount = await sslStream.ReadAsync(bytes, 0, bytes.Length); }
                catch (IOException) { break; } // Timeout Occurred, aka no data left to read

                if (byteCount > 0) contentBuilder.Append(Encoding.UTF8.GetString(bytes, 0, byteCount));

            } while (byteCount > 0);

            return contentBuilder.ToString();
        }
    }
}
