using Microsoft.Extensions.Logging;
using System;

namespace Netler.Contracts
{
    /// <summary>
    /// Configuration parameters for starting a Netler Server.
    /// Pass an <see cref="System.Action{IConfiguration}"/> to <c>Server.Create</c> to configure the server.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Sets the TCP port the server listens on.
        /// </summary>
        /// <param name="port">A valid TCP port number (1–65535).</param>
        /// <example>
        /// <code>
        /// Server.Create(config =>
        /// {
        ///     config.UsePort(5544);
        /// });
        /// </code>
        /// </example>
        void UsePort(int port);

        /// <summary>
        /// Monitors the OS process with the given PID and reacts when it exits.
        /// The reaction is controlled by <see cref="UseClientDisconnectBehaviour"/>;
        /// by default the entire application is shut down.
        /// </summary>
        /// <param name="pid">The OS process ID of the client process to monitor.</param>
        /// <example>
        /// <code>
        /// Server.Create(config =>
        /// {
        ///     config.UsePort(5544);
        ///     config.UseClientPid(clientProcessId);
        ///     config.UseClientDisconnectBehaviour(ClientDisconnectBehaviour.DisposeServer);
        /// });
        /// </code>
        /// </example>
        void UseClientPid(int pid);

        /// <summary>
        /// Configures what the server does when the monitored client process exits.
        /// Only meaningful when <see cref="UseClientPid"/> is also called.
        /// </summary>
        /// <param name="behaviour">The disconnect behaviour to apply.</param>
        /// <example>
        /// <code>
        /// config.UseClientDisconnectBehaviour(ClientDisconnectBehaviour.DisposeServer);
        /// </code>
        /// </example>
        void UseClientDisconnectBehaviour(ClientDisconnectBehaviour behaviour);

        /// <summary>
        /// Registers the route table the server exposes to clients.
        /// </summary>
        /// <param name="routes">
        /// A callback that receives an <see cref="IRoutes"/> builder and registers one or
        /// more named route handlers.
        /// </param>
        /// <example>
        /// <code>
        /// config.UseRoutes(routes =>
        /// {
        ///     // Typed — types are inferred from the lambda
        ///     routes.AddTyped("Add",  (int a, int b) => a + b);
        ///     routes.AddTyped("Ping", () => "pong");
        ///
        ///     // Raw — manual parameter decoding
        ///     routes.Add("Multiply", param =>
        ///     {
        ///         var a = Convert.ToInt32(param[0]);
        ///         var b = Convert.ToInt32(param[1]);
        ///         return a * b;
        ///     });
        /// });
        /// </code>
        /// </example>
        void UseRoutes(Action<IRoutes> routes);

        /// <summary>
        /// Attaches an <see cref="ILogger"/> for server diagnostics.
        /// When not called, logging is silently discarded via <c>NullLogger</c>.
        /// </summary>
        /// <param name="logger">The logger instance to write to.</param>
        /// <example>
        /// <code>
        /// ILogger logger = loggerFactory.CreateLogger&lt;MyApp&gt;();
        /// config.UseLogger(logger);
        /// </code>
        /// </example>
        void UseLogger(ILogger logger);

        /// <summary>
        /// Returns the configured <see cref="ClientDisconnectBehaviour"/>, or <c>null</c> if not set.
        /// </summary>
        ClientDisconnectBehaviour? GetClientDisconnectBehaviour();

        /// <summary>
        /// Returns the configured <see cref="IRoutes"/> instance.
        /// </summary>
        IRoutes GetRoutes();

        /// <summary>
        /// Returns the configured TCP port.
        /// </summary>
        int GetPort();

        /// <summary>
        /// Returns the configured client process ID, or <c>null</c> if not set.
        /// </summary>
        int? GetClientPid();

        /// <summary>
        /// Returns the configured <see cref="ILogger"/>. Defaults to <c>NullLogger</c>.
        /// </summary>
        ILogger GetLogger();
    }
}
