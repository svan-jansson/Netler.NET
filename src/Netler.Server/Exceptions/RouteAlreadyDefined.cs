using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to add a <see cref="Netler.Routes"/> route and it already exists in the route table
    /// </summary>
    [Serializable()]
    public class RouteAlreadyDefined : Exception
    {
        public RouteAlreadyDefined() : base() { }
        public RouteAlreadyDefined(string message) : base(message) { }
        public RouteAlreadyDefined(string message, Exception inner) : base(message, inner) { }
        protected RouteAlreadyDefined(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}