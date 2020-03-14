using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown from the <see cref="Client"/> when the remote invokation throws an exception on the server
    /// </summary>
    [Serializable]
    public class RemoteInvokationFailed : Exception
    {
        public RemoteInvokationFailed() : base() { }
        public RemoteInvokationFailed(string message) : base(message) { }
        public RemoteInvokationFailed(string message, Exception inner) : base(message, inner) { }
        protected RemoteInvokationFailed(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}