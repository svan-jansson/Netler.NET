using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Netler
{
    internal static class StreamExtensions
    {
        const int HeaderSize = 4;

        internal static byte[] ReadWithHeader(this NetworkStream stream)
        {
            var header = new byte[HeaderSize];
            ReadExactly(stream, header, HeaderSize);
            Array.Reverse(header);
            var contentLength = BitConverter.ToInt32(header, 0);
            var content = new byte[contentLength];
            ReadExactly(stream, content, contentLength);
            return content;
        }

        internal static void WriteWithHeader(this NetworkStream stream, byte[] content)
        {
            var header = BitConverter.GetBytes(content.Length);
            Array.Reverse(header);
            var packet = new byte[HeaderSize + content.Length];
            header.CopyTo(packet, 0);
            content.CopyTo(packet, HeaderSize);
            stream.Write(packet, 0, packet.Length);
        }

        internal static async Task<byte[]> ReadWithHeaderAsync(this NetworkStream stream, CancellationToken cancellationToken = default)
        {
            var header = new byte[HeaderSize];
            await ReadExactlyAsync(stream, header, HeaderSize, cancellationToken);
            Array.Reverse(header);
            var contentLength = BitConverter.ToInt32(header, 0);
            var content = new byte[contentLength];
            await ReadExactlyAsync(stream, content, contentLength, cancellationToken);
            return content;
        }

        internal static async Task WriteWithHeaderAsync(this NetworkStream stream, byte[] content, CancellationToken cancellationToken = default)
        {
            var header = BitConverter.GetBytes(content.Length);
            Array.Reverse(header);
            var packet = new byte[HeaderSize + content.Length];
            header.CopyTo(packet, 0);
            content.CopyTo(packet, HeaderSize);
            await stream.WriteAsync(packet, 0, packet.Length, cancellationToken);
        }

        private static void ReadExactly(NetworkStream stream, byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0) throw new EndOfStreamException();
                offset += read;
            }
        }

        private static async Task ReadExactlyAsync(NetworkStream stream, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = await stream.ReadAsync(buffer, offset, count - offset, cancellationToken);
                if (read == 0) throw new EndOfStreamException();
                offset += read;
            }
        }
    }
}
