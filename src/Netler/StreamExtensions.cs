using System;
using System.Net.Sockets;

namespace Netler
{
    internal static class StreamExtensions
    {
        const int HeaderSize = 4;

        internal static byte[] ReadWithHeader(this NetworkStream stream)
        {
            var header = new byte[HeaderSize];
            stream.Read(header, 0, HeaderSize);
            Array.Reverse(header);
            var contentLength = BitConverter.ToInt32(header, 0);
            var content = new byte[contentLength];
            stream.Read(content, 0, contentLength);
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
    }
}
