using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to invoke a method on a <see cref="Netler.Routes"/> route and it doesn't exist in the route table
    /// </summary>
    [Serializable()]
    public class RouteUndefined : Exception
    {
        public RouteUndefined() : base() { }
        public RouteUndefined(string message) : base(message) { }
        public RouteUndefined(string message, Exception inner) : base(message, inner) { }
        protected RouteUndefined(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}