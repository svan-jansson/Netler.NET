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
    /// A Netler Server listens to incoming TCP requests and translates them into method calls.
    /// </summary>
    /// <example>
    /// <code>
    /// var server = Server.Create(config =>
    /// {
    ///     config.UsePort(5544);
    ///     config.UseRoutes(routes =>
    ///     {
    ///         routes.AddTyped("Add", (int a, int b) => a + b);
    ///         routes.AddTyped("Ping", () => "pong");
    ///     });
    /// });
    ///
    /// await server.Start();
    /// </code>
    /// </example>
    public partial class Server
    {
        private readonly IConfiguration _configuration;
        private CancellationTokenSource _cancellationSource;

        private Server()
        {
            _configuration = new Configuration();
        }

        /// <summary>
        /// Creates a new Netler Server instance with the provided configuration.
        /// </summary>
        /// <param name="configure">A callback that configures the server (port, routes, logger, etc.).</param>
        /// <returns>A configured <see cref="Server"/> instance, ready to be started.</returns>
        /// <example>
        /// <code>
        /// var server = Server.Create(config =>
        /// {
        ///     config.UsePort(5544);
        ///     config.UseRoutes(routes =>
        ///     {
        ///         routes.AddTyped("Add",    (int a, int b) => a + b);
        ///         routes.AddTyped("Double", (int x) => x * 2);
        ///         routes.AddTyped("Ping",   () => "pong");
        ///     });
        /// });
        /// </code>
        /// </example>
        public static Server Create(Action<IConfiguration> configure)
        {
            var server = new Server();
            configure(server._configuration);
            return server;
        }

        /// <summary>
        /// Starts the Netler Server and blocks until <see cref="Stop"/> is called or the
        /// <paramref name="cancellationToken"/> is signalled.
        /// </summary>
        /// <param name="cancellationToken">
        /// An optional token that cancels the server loop. When cancelled the server stops
        /// accepting new connections and returns.
        /// </param>
        /// <returns>A <see cref="Task{Server}"/> that completes when the server stops, yielding this instance.</returns>
        /// <example>
        /// <code>
        /// // Fire-and-forget — stop later via server.Stop()
        /// var serverTask = server.Start();
        ///
        /// // Or cancel via a token
        /// using var cts = new CancellationTokenSource();
        /// var serverTask = server.Start(cts.Token);
        /// cts.CancelAfter(TimeSpan.FromMinutes(5));
        /// await serverTask;
        /// </code>
        /// </example>
        public Task<Server> Start(CancellationToken cancellationToken = default)
        {
            _cancellationSource?.Dispose();
            _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            return Task.Run(() => StartServerAsync(_cancellationSource.Token), _cancellationSource.Token);
        }

        /// <summary>
        /// Stops the Netler Server. Any in-flight request will be allowed to complete before
        /// the server loop exits.
        /// </summary>
        /// <returns>This <see cref="Server"/> instance, enabling a fluent call chain.</returns>
        /// <example>
        /// <code>
        /// var serverTask = server.Start();
        /// // ... do work ...
        /// server.Stop();
        /// await serverTask;
        /// </code>
        /// </example>
        public Server Stop()
        {
            try { _cancellationSource?.Cancel(); }
            catch (ObjectDisposedException) { }
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
            try
            {
                LogListening(logger, port);

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
#if NET8_0_OR_GREATER
                    tcpClient = await listener.AcceptTcpClientAsync(ct).ConfigureAwait(false);
#else
                    // Optimization: dispose the registration once AcceptTcpClientAsync returns
                    // so the CT callback cannot fire after the listener is already in use.
                    using var registration = ct.Register(() => listener.Stop());
                    tcpClient = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
#endif
                }
                catch (SocketException) when (ct.IsCancellationRequested)
                {
                    LogStoppedBeforeConnect(logger);
                    return this;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    LogStoppedBeforeConnect(logger);
                    return this;
                }

                LogClientConnected(logger);

                using var client = tcpClient;
                var stream = client.GetStream();

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var encodedRequest = await stream.ReadWithHeaderAsync(ct).ConfigureAwait(false);
                        var request = Request.Decode(encodedRequest);

                        LogRequest(logger, request.Route);

                        try
                        {
                            var methodResponse = routes.Invoke(request.Route, request.Parameters);
                            var response = new Response(Response.Code.Ok, methodResponse);
                            await stream.WriteWithHeaderAsync(response.Encode(), ct).ConfigureAwait(false);
                        }
                        catch (RouteMethodCallFailed ex)
                        {
                            LogRouteError(logger, ex, request.Route);
                            var response = new Response(Response.Code.Error, ex.InnerException.Message);
                            await stream.WriteWithHeaderAsync(response.Encode(), ct).ConfigureAwait(false);
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
                        LogUnexpectedError(logger, ex);
                        break;
                    }
                }

                LogServerStopped(logger);
                return this;
            }
            finally
            {
                listener.Stop();
                _cancellationSource?.Dispose();
            }
        }

        private void StartCheckingIfClientIsAlive(int clientPid, ClientDisconnectBehaviour behaviour, CancellationToken ct, ILogger logger)
        {
            _ = Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested && ClientIsAlive(clientPid))
                {
                    try { await Task.Delay(500, ct).ConfigureAwait(false); }
                    catch (OperationCanceledException) { return; }
                }

                if (ct.IsCancellationRequested) return;

                LogClientDisconnected(logger, clientPid, behaviour);

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
        }

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

        // ── [LoggerMessage] source-generated log delegates ───────────────────────
        // Pre-compiled at build time by the Microsoft.Extensions.Logging.Abstractions
        // source generator — no runtime string-format parsing, no argument boxing.

        [LoggerMessage(Level = LogLevel.Information, Message = "Netler server listening on port {Port}")]
        private static partial void LogListening(ILogger logger, int port);

        [LoggerMessage(Level = LogLevel.Information, Message = "Netler server stopped before accepting a connection")]
        private static partial void LogStoppedBeforeConnect(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Client connected")]
        private static partial void LogClientConnected(ILogger logger);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Received request for route {Route}")]
        private static partial void LogRequest(ILogger logger, string route);

        [LoggerMessage(Level = LogLevel.Error, Message = "Route {Route} threw an exception")]
        private static partial void LogRouteError(ILogger logger, Exception exception, string route);

        [LoggerMessage(Level = LogLevel.Error, Message = "Unexpected error in server loop")]
        private static partial void LogUnexpectedError(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Information, Message = "Netler server stopped")]
        private static partial void LogServerStopped(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Client process {Pid} has disconnected. Behaviour: {Behaviour}")]
        private static partial void LogClientDisconnected(ILogger logger, int pid, ClientDisconnectBehaviour behaviour);
    }
}
