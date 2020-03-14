using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to invoke a method on a <see cref="Netler.Routes"/> route and it doesn't exist in the route table
    /// </summary>
    [Serializable()]
    public class RouteUndefined : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception with a message describing the details of the error
        /// </summary>
        /// <param name="message">A message describing the error</param>
        public RouteUndefined(string message) : base(message) { }
    }
}