using Netler.Contracts;
using Netler.Exceptions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Netler
{
    /// <summary>
    /// A Netler Server listens to incoming TCP requests and translates them into method calls
    /// </summary>
    public class Server
    {
        private IConfiguration _configuration;
        private CancellationTokenSource _cancellationSource;
        private CancellationToken _cancellationToken;

        private Server()
        {
            _configuration = new Configuration();
        }

        /// <summary>
        /// Creates new Netler Server instance
        /// </summary>
        /// <param name="port">Which port the server will listen to</param>
        public static Server Create(Action<IConfiguration> configure)
        {
            var server = new Server();
            configure(server._configuration);
            return server;
        }

        /// <summary>
        /// Starts a process running the Netler Server
        /// </summary>
        public Task<Server> StartAsync()
        {
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            var server = Task.Run(StartServer, _cancellationToken);

            return server;
        }

        /// <summary>
        /// Stops the Netler Server
        /// </summary>
        public Server Stop()
        {
            _cancellationSource.Cancel();
            return this;
        }

        private Server StartServer()
        {
            var port = _configuration.GetPort();
            var routes = _configuration.GetRoutes();

            var localhost = IPAddress.Parse("127.0.0.1");
            var listener = new TcpListener(localhost, port);

            listener.Start();
            var client = listener.AcceptTcpClient();
            var stream = client.GetStream();

            while (!_cancellationToken.IsCancellationRequested)
            {
                if (!stream.DataAvailable)
                {
                    Task.Delay(1).GetAwaiter().GetResult();
                }
                else
                {
                    var encodedRequest = new byte[client.Available];
                    stream.Read(encodedRequest, 0, encodedRequest.Length);

                    var request = Request.Decode(encodedRequest);

                    try
                    {
                        var methodResponse = routes.Invoke(request.Route, request.Parameters);
                        var response = new Response(Response.Code.Ok, methodResponse);
                        var responseBytes = response.Encode();
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }
                    catch (RouteMethodCallFailed ex)
                    {
                        var response = new Response(Response.Code.Error, ex.InnerException.Message);
                        var responseBytes = response.Encode();
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }
                }
            }

            listener.Stop();
            return this;
        }
    }
}
