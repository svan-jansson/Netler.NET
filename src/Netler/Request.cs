using System.Collections.Generic;

namespace Netler
{
    /// <summary>
    /// A request from a Netler Client to a Netler Server
    /// </summary>
    public struct Request
    {
        public Request(string route, object[] parameters)
        {
            Route = route;
            Parameters = parameters;
        }

        /// <summary>
        /// The server route of the request
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// The parameters for the method at the server route
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        /// Encodes the request to the Netler transport format
        /// </summary>
        /// <returns>A byte array that can be parsed by a Netler server</returns>
        public byte[] Encode() => Message.Encode(new Dictionary<string, object>
        {
            { "name", Route },
            { "params", Parameters }
        });

        /// <summary>
        /// Decodes a request from the Netler transport format
        /// </summary>
        /// /// <returns>A byte array containing a Netler request encoded in the Netler transport format</returns>
        public static Request Decode(byte[] bytes)
        {
            var decoded = Message.Decode<Dictionary<string, object>>(bytes);
            return new Request
            {
                Route = decoded["name"].ToString(),
                Parameters = decoded["params"] as object[]
            };
        }

        public override bool Equals(object other) => Equals((Request)other);

        public override int GetHashCode() => (Route, Parameters).GetHashCode();

        public static bool operator ==(Request first, Request second) => Equals(first, second);

        public static bool operator !=(Request first, Request second) => !(first == second);
    }
}
