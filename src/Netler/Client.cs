﻿using Netler.Exceptions;
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
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;

        /// <summary>
        /// Create a new client to a localhost server listing on a given TCP port
        /// </summary>
        /// <param name="port">A valid TCP port</param>
        public Client(int port)
        {
            _tcpClient = new TcpClient("localhost", port);
            _stream = _tcpClient.GetStream();
        }

        /// <summary>
        /// Create a new client to a named server listing on a given TCP port
        /// </summary>
        /// <param name="port">A valid TCP port</param>
        /// <param name="hostname">A valid hostname</param>
        public Client(string hostname, int port)
        {
            _tcpClient = new TcpClient(hostname, port);
            _stream = _tcpClient.GetStream();
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

            _stream.Write(encoded, 0, encoded.Length);

            var responseBytes = new List<byte>();
            var buffer = new byte[256];

            do
            {
                Thread.Sleep(20);
            } while (!_stream.DataAvailable);

            if (_stream.DataAvailable && _stream.CanRead)
            {
                while (_stream.DataAvailable)
                {
                    _stream.Read(buffer, 0, buffer.Length);
                    responseBytes.AddRange(buffer);
                }

                response = Response.Decode(responseBytes.ToArray());

                if (response.Status == Response.Code.Error)
                {
                    throw new RemoteInvokationFailed($"Method at route {route} threw an error: {response.Data}");
                }
            }

            return response.Data;
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
            _tcpClient.Dispose();
        }
    }
}
