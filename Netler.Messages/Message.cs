using MessagePack;
using Netler.Exceptions;

namespace Netler
{
    /// <summary>
    /// Module for encoding/decoding messages for use with Netler clients and servers
    /// </summary>
    internal class Message
    {
        public static byte[] Encode(object data)
        {
            try
            {
                return MessagePackSerializer.Serialize(data);
            }
            catch (MessagePackSerializationException)
            {
                throw new InvalidDataType($"The data type {data.GetType().Name} cannot be encoded. Try using an anonymous type.");
            }
        }

        public static object Decode(byte[] data) => MessagePackSerializer.Deserialize<object>(data);

        public static T Decode<T>(byte[] data) => MessagePackSerializer.Deserialize<T>(data);


    }
}
