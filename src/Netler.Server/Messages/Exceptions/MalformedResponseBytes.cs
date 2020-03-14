using System;

namespace Netler.Exceptions
{
    /// <summary>
    /// Thrown when trying to decode a <see cref="Response"/> from a byte array that is malformed
    /// </summary>
    [Serializable]
    internal class MalformedResponseBytes : Exception
    {
        public MalformedResponseBytes() : base() { }
        public MalformedResponseBytes(string message) : base(message) { }
        public MalformedResponseBytes(string message, Exception inner) : base(message, inner) { }
        protected MalformedResponseBytes(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}