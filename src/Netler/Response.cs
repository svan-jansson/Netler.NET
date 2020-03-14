using Netler.Exceptions;
using Netler.Reflection;
using System;

namespace Netler
{
    /// <summary>
    /// A response from a Netler Server
    /// </summary>
    public struct Response
    {
        /// <summary>
        /// The status of a Netler response
        /// </summary>
        public enum Code
        {
            Error = 0,
            Ok = 1
        }

        /// <summary>
        /// Creates a <see cref="Response"/> with a status and an anonymously typed data payload
        /// </summary>
        /// <param name="status">The response status code</param>
        /// <param name="data">The anonymous data payload</param>
        public Response(Code status, object data)
        {
            Status = status;
            Data = data;
        }

        /// <summary>
        /// Creates a <see cref="Response"/> with a status and an named data payload.
        /// Using this constructor has a major impact on transport performance, as it uses reflection to convert the type into a serializable anonymous object.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="data"/> parameter
        /// <param name="status">The response status code</param>
        /// <param name="data">The data payload</param>
        public static Response FromNamed<T>(Code status, T data)
        {
            var anonymousData = data.ToAnonymous();
            return new Response(status, anonymousData);
        }

        /// <summary>
        /// The status code of the message ivokation at the Netler server
        /// </summary>
        public Code Status { get; set; }

        /// <summary>
        /// The response data payload
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Encodes the response to the Netler transport format
        /// </summary>
        /// <returns>A byte array that can be parsed by a Netler server</returns>
        public byte[] Encode() =>
                Message.Encode(new object[]
                {
                    Status,
                    Data
                });


        /// <summary>
        /// Decodes a response from the Netler transport format
        /// </summary>
        /// /// <returns>A byte array containing a Netler Response encoded in the Netler transport format</returns>
        public static Response Decode(byte[] bytes)
        {
            try
            {
                var decoded = Message.Decode<object[]>(bytes);
                return new Response
                {
                    Status = (Code)Convert.ToInt32(decoded[0]),
                    Data = decoded[1] as object
                };
            }
            catch (IndexOutOfRangeException)
            {
                throw new MalformedResponseBytes("Input byte array is malformed");
            }
        }

        public override bool Equals(object other) => Equals((Response)other);

        public override int GetHashCode() => (Status, Data).GetHashCode();

        public static bool operator ==(Response first, Response second) => Equals(first, second);

        public static bool operator !=(Response first, Response second) => !(first == second);

    }

}
