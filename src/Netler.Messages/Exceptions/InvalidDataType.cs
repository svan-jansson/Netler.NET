using System;

namespace Netler.Exceptions
{
    [Serializable]
    internal class InvalidDataType : Exception
    {
        public InvalidDataType() : base() { }
        public InvalidDataType(string message) : base(message) { }
        public InvalidDataType(string message, Exception inner) : base(message, inner) { }
        protected InvalidDataType(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}