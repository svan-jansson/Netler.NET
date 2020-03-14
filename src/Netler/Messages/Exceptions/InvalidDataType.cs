using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to encode an object to the Netler binary message format and it fails
    /// </summary>
    [Serializable]
    internal class InvalidDataType : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception with a message describing the details of the error
        /// </summary>
        /// <param name="message">A message describing the error</param>
        public InvalidDataType(string message) : base(message) { }
    }
}