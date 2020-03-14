using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when an invoking a method on a <see cref="Netler.Routes"/> route and it throws any unhandled exception
    /// </summary>
    [Serializable()]
    public class RouteMethodCallFailed : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception with a message describing the details of the error
        /// </summary>
        /// <param name="message">A message describing the error</param>
        /// <param name="inner">An inner exception containing details of the caught error</param>
        public RouteMethodCallFailed(string message, Exception inner) : base(message, inner) { }
    }
}