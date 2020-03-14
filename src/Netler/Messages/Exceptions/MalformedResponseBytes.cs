using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to decode a <see cref="Response"/> from a byte array that is malformed
    /// </summary>
    [Serializable]
    internal class MalformedResponseBytes : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception with a message describing the details of the error
        /// </summary>
        /// <param name="message">A message describing the error</param>
        public MalformedResponseBytes(string message) : base(message) { }
    }
}