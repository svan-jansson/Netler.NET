using System;

namespace Netler.Contracts
{
    /// <summary>
    /// Message routing for a Netler Server
    /// </summary>
    public interface IRoutes
    {
        /// <summary>
        /// Adds a new route to the route table
        /// </summary>
        /// <param name="route">The exposed name of the route. Example: "CreateNewFoo"</param>
        /// <param name="method">A method to execute for calls to the route</param>
        void Add(string route, Func<object[], object> method);

        /// <summary>
        /// Invokes the method that is linked to a route
        /// </summary>
        /// <param name="route">The exposed name of the route. Example: "CreateNewFoo"</param>
        /// <param name="parameters">A list of parameters to pass to the method</param>
        object Invoke(string route, object[] parameters);
    }
}