using Netler.Contracts;
using Netler.Exceptions;
using System;
using System.Collections.Generic;

namespace Netler
{
    /// <summary>
    /// Message routing for a Netler Server
    /// </summary>
    internal class Routes : IRoutes
    {
        private readonly IDictionary<string, Func<object[], object>> _routeTable;

        internal Routes()
        {
            _routeTable = new Dictionary<string, Func<object[], object>>();
        }

        /// <summary>
        /// Adds a new route to the route table
        /// </summary>
        /// <param name="route">The exposed name of the route. Example: "CreateNewFoo"</param>
        /// <param name="method">A method to execute for calls to the route</param>
        public void Add(string route, Func<object[], object> method)
        {
            if (!_routeTable.ContainsKey(route))
            {
                _routeTable.Add(route, method);
            }
            else
            {
                throw new RouteAlreadyDefined($"Route already exists in the route map: {route}");
            }
        }

        /// <summary>
        /// Invokes the method that is linked to a route
        /// </summary>
        /// <param name="route">The exposed name of the route. Example: "CreateNewFoo"</param>
        /// <param name="parameters">A list of parameters to pass to the method</param>
        public object Invoke(string route, object[] parameters)
        {
            if (!_routeTable.ContainsKey(route))
            {
                throw new RouteUndefined($"Route is not defined in the route map: {route}");
            }
            var invokeMethod = _routeTable[route];
            try
            {
                return invokeMethod(parameters);
            }
            catch (Exception ex)
            {
                throw new RouteMethodCallFailed($"An exception was thrown when executing method on route {route}", ex);
            }
        }
    }
}