using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown from the <see cref="Client"/> when the remote invokation throws an exception on the server
    /// </summary>
    [Serializable]
    public class RemoteInvokationFailed : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception with a message describing the details of the error
        /// </summary>
        /// <param name="message">A message describing the error</param>
        public RemoteInvokationFailed(string message) : base(message) { }
    }
}