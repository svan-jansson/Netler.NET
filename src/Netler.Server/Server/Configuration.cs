using Netler.Contracts;
using System;

namespace Netler
{
    /// <summary>
    /// Default configuration parameters for a Netler Server
    /// </summary>
    internal class Configuration : IConfiguration
    {
        private int _port;
        private int? _clientPid;
        private ClientDisconnectBehaviour? _clientDisconnectBehaviour;
        private IRoutes _routes;

        public void UseClientPid(int pid)
        {
            _clientPid = pid;
            _clientDisconnectBehaviour = ClientDisconnectBehaviour.ShutdownApplication;
        }

        public void UseClientDisconnectBehaviour(ClientDisconnectBehaviour behaviour)
        {
            _clientDisconnectBehaviour = behaviour;
        }

        public void UsePort(int port)
        {
            _port = port;
        }

        public void UseRoutes(Action<IRoutes> routes)
        {
            _routes = new Routes();
            routes(_routes);
        }

        public int GetPort() => _port;

        public int? GetClientPid() => _clientPid;

        public ClientDisconnectBehaviour? GetClientDisconnectBehaviour() => _clientDisconnectBehaviour;

        public IRoutes GetRoutes() => _routes;
    }
}