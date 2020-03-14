using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to add a <see cref="Netler.Routes"/> route and it already exists in the route table
    /// </summary>
    [Serializable()]
    public class RouteAlreadyDefined : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception with a message describing the details of the error
        /// </summary>
        /// <param name="message">A message describing the error</param>
        public RouteAlreadyDefined(string message) : base(message) { }
    }
}