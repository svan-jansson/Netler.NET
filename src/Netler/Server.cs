using Netler.Contracts;
using Netler.Exceptions;
using System;
using System.Diagnostics;
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
        public Task<Server> Start()
        {
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            return Task.Run(StartServer, _cancellationToken);
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
            var clientPid = _configuration.GetClientPid();

            var localhost = IPAddress.Parse("127.0.0.1");
            var listener = new TcpListener(localhost, port);

            if (clientPid != null)
            {
                StartCheckIfClientIsAlive(
                    listener,
                    (int)clientPid,
                    (ClientDisconnectBehaviour)_configuration.GetClientDisconnectBehaviour());
            }

            listener.Start();
            var client = listener.AcceptTcpClient();
            var stream = client.GetStream();

            while (!_cancellationSource.IsCancellationRequested)
            {
                if (!stream.DataAvailable)
                {
                    Tick(1);
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

        private void Tick(int ms) => Task.Delay(ms).GetAwaiter().GetResult();

        private void StartCheckIfClientIsAlive(TcpListener listener, int clientPid, ClientDisconnectBehaviour behaviour)
            => Task.Run(() =>
            {
                while (!_cancellationSource.IsCancellationRequested && ClientIsAlive(clientPid))
                {
                    Tick(500);
                }

                if (_cancellationSource.IsCancellationRequested)
                {
                    return;
                }

                switch (behaviour)
                {
                    case ClientDisconnectBehaviour.ShutdownApplication:
                        Stop();
                        Environment.Exit(0);
                        break;
                    case ClientDisconnectBehaviour.DisposeServer:
                        Stop();
                        break;
                    case ClientDisconnectBehaviour.KeepAlive:
                        break;
                }
            });

        private bool ClientIsAlive(int clientPid)
        {
            try
            {
                var process = Process.GetProcessById(clientPid);
                return !process.HasExited;
            }
            catch
            {
                return false;
            }
        }

    }
}
