using System.Collections.Generic;

namespace Netler
{
    /// <summary>
    /// A request from a Netler Client to a Netler Server
    /// </summary>
    public struct Request
    {
        /// <summary>
        /// Creates a new request value object
        /// </summary>
        /// <param name="route">The route for the request</param>
        /// <param name="parameters">The parameters to pass to the method invokation</param>
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

        /// <summary>
        /// <inheritdoc cref="object.Equals(object)"/>
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>True if the objects are equal</returns>
        public override bool Equals(object other) => Equals((Request)other);

        /// <summary>
        /// <inheritdoc cref="object.GetHashCode()"/>
        /// </summary>
        public override int GetHashCode() => (Route, Parameters).GetHashCode();

        /// <summary>
        /// <inheritdoc cref="Equals(object)"/>
        /// </summary>
        public static bool operator ==(Request first, Request second) => Equals(first, second);

        /// <summary>
        /// Determines whether the specified object is different from the current object
        /// </summary>
        public static bool operator !=(Request first, Request second) => !(first == second);
    }
}
