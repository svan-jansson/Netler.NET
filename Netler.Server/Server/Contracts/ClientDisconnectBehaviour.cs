namespace Netler.Contracts
{
    /// <summary>
    /// Describes what should happen when the connected client disconnects from the server
    /// </summary>
    public enum ClientDisconnectBehaviour
    {
        /// <summary>
        /// Shuts down the entire application that's hosting the server (default)
        /// </summary>
        ShutdownApplication,

        /// <summary>
        /// Disposes of the current Netler server instance but keeps the application running
        /// </summary>
        DisposeServer,

        /// <summary>
        /// Keeps the server alive to accept new client connections
        /// </summary>
        KeepAlive
    }
}