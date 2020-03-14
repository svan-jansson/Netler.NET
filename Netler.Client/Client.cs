using Netler.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Netler
{
    /// <summary>
    /// Client for calling a Netler server over a specific TCP port
    /// </summary>
    public class Client : IDisposable
    {
        TcpClient _tcpClient;

        public Client(int port)
        {
            _tcpClient = new TcpClient("localhost", port);
        }

        /// <summary>
        /// Invokes a method on the Netler server using its route
        /// </summary>
        /// <param name="route">The name of the route</param>
        /// <param name="parameters">The parameters to pass to the method</param>
        /// <returns></returns>
        public object Invoke(string route, object[] parameters)
        {
            var message = new Request(route, parameters);

            var encoded = message.Encode();
            var response = default(Response);

            using (var stream = _tcpClient.GetStream())
            {
                stream.Write(encoded, 0, encoded.Length);

                var responseBytes = new List<byte>();
                var buffer = new byte[256];
                var responseLength = 0;

                do
                {
                    Thread.Sleep(20);
                } while (!stream.DataAvailable);

                if (stream.DataAvailable && stream.CanRead)
                {
                    while (stream.DataAvailable)
                    {
                        responseLength = stream.Read(buffer, 0, buffer.Length);
                        responseBytes.AddRange(buffer);
                    }

                    response = Response.Decode(responseBytes.ToArray());
                    if (response.Status == Response.Code.Error)
                    {
                        throw new RemoteInvokationFailed($"Method at route {route} threw an error: {response.Data}");
                    }
                }

            }

            return response.Data;
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}
