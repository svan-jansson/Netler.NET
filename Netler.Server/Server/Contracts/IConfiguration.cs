using System;

namespace Netler.Contracts
{
    /// <summary>
    /// Configuration parameters for starting a Netler Server
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Which port the Netler Server should listen to
        /// </summary>
        void UsePort(int port);

        /// <summary>
        /// By passing a client OS pid the Netler Server will automatically shut down when the client shuts down
        /// </summary>
        void UseClientPid(int pid);

        /// <summary>
        /// By passing a client OS pid the Netler Server will automatically react when a client disconnects
        /// (default by shutting down the entire application). Use <seealso cref="UseClientDisconnectBehaviour"/> to change the behaviour.
        /// </summary>
        void UseClientDisconnectBehaviour(ClientDisconnectBehaviour behaviour);

        /// <summary>
        /// Which routes the Netler Server should expose
        /// </summary>
        /// <param name="routes">A mapping of string routes to methods that are executed when the route is called</param>
        void UseRoutes(Action<IRoutes> routes);

        /// <summary>
        /// The currently configured client disconnect behaviour
        /// </summary>
        ClientDisconnectBehaviour? GetClientDisconnectBehaviour();

        /// <summary>
        /// The currently configured routes
        /// </summary>
        IRoutes GetRoutes();

        /// <summary>
        /// The currently configured port
        /// </summary>
        int GetPort();

        /// <summary>
        /// The currently configured client pid
        /// </summary>
        int? GetClientPid();

    }
}