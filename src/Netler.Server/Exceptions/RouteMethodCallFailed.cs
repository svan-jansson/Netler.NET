using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when an invoking a method on a <see cref="Netler.Routes"/> route and it throws any unhandled exception
    /// </summary>
    [Serializable()]
    public class RouteMethodCallFailed : Exception
    {
        public RouteMethodCallFailed() : base() { }
        public RouteMethodCallFailed(string message) : base(message) { }
        public RouteMethodCallFailed(string message, Exception inner) : base(message, inner) { }
        protected RouteMethodCallFailed(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}