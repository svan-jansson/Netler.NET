using Microsoft.Extensions.Logging;
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
        private readonly IConfiguration _configuration;
        private CancellationTokenSource _cancellationSource;

        private Server()
        {
            _configuration = new Configuration();
        }

        /// <summary>
        /// Creates new Netler Server instance
        /// </summary>
        /// <param name="configure">Callback for configuring the server instance</param>
        public static Server Create(Action<IConfiguration> configure)
        {
            var server = new Server();
            configure(server._configuration);
            return server;
        }

        /// <summary>
        /// Starts the Netler Server
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the server</param>
        public Task<Server> Start(CancellationToken cancellationToken = default)
        {
            _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            return Task.Run(() => StartServerAsync(_cancellationSource.Token), _cancellationSource.Token);
        }

        /// <summary>
        /// Stops the Netler Server
        /// </summary>
        public Server Stop()
        {
            _cancellationSource?.Cancel();
            return this;
        }

        private async Task<Server> StartServerAsync(CancellationToken ct)
        {
            var port = _configuration.GetPort();
            var routes = _configuration.GetRoutes();
            var clientPid = _configuration.GetClientPid();
            var logger = _configuration.GetLogger();

            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            ct.Register(() => listener.Stop());

            logger.LogInformation("Netler server listening on port {Port}", port);

            if (clientPid != null)
            {
                StartCheckingIfClientIsAlive(
                    (int)clientPid,
                    (ClientDisconnectBehaviour)_configuration.GetClientDisconnectBehaviour(),
                    ct,
                    logger);
            }

            TcpClient tcpClient;
            try
            {
                tcpClient = await listener.AcceptTcpClientAsync();
            }
            catch (SocketException) when (ct.IsCancellationRequested)
            {
                logger.LogInformation("Netler server stopped before accepting a connection");
                return this;
            }

            logger.LogInformation("Client connected");

            using (tcpClient)
            {
                var stream = tcpClient.GetStream();

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var encodedRequest = await stream.ReadWithHeaderAsync(ct);
                        var request = Request.Decode(encodedRequest);

                        logger.LogDebug("Received request for route {Route}", request.Route);

                        try
                        {
                            var methodResponse = routes.Invoke(request.Route, request.Parameters);
                            var response = new Response(Response.Code.Ok, methodResponse);
                            await stream.WriteWithHeaderAsync(response.Encode(), ct);
                        }
                        catch (RouteMethodCallFailed ex)
                        {
                            logger.LogError(ex, "Route {Route} threw an exception", request.Route);
                            var response = new Response(Response.Code.Error, ex.InnerException.Message);
                            await stream.WriteWithHeaderAsync(response.Encode(), ct);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (System.IO.EndOfStreamException)
                    {
                        // Client closed the connection
                        break;
                    }
                    catch (Exception ex) when (!ct.IsCancellationRequested)
                    {
                        logger.LogError(ex, "Unexpected error in server loop");
                        break;
                    }
                }
            }

            logger.LogInformation("Netler server stopped");
            return this;
        }

        private void StartCheckingIfClientIsAlive(int clientPid, ClientDisconnectBehaviour behaviour, CancellationToken ct, ILogger logger)
            => Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested && ClientIsAlive(clientPid))
                {
                    try { await Task.Delay(500, ct); }
                    catch (OperationCanceledException) { return; }
                }

                if (ct.IsCancellationRequested) return;

                logger.LogInformation("Client process {Pid} has disconnected. Behaviour: {Behaviour}", clientPid, behaviour);

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
            }, ct);

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
