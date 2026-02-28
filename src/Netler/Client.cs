using Netler.Exceptions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Netler
{
    /// <summary>
    /// Client for calling a Netler server over TCP.
    /// </summary>
    /// <example>
    /// <code>
    /// using var client = new Client(5544);
    ///
    /// // Typed async call — return value is coerced to T automatically
    /// var sum  = await client.InvokeAsync&lt;int&gt;("Add", new object[] { 2, 3 });
    /// var pong = await client.InvokeAsync&lt;string&gt;("Ping", new object[0]);
    /// </code>
    /// </example>
    public class Client : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;

        /// <summary>
        /// Creates a client connected to a Netler server on <c>localhost</c>.
        /// </summary>
        /// <param name="port">The TCP port the server is listening on.</param>
        /// <example>
        /// <code>
        /// using var client = new Client(5544);
        /// </code>
        /// </example>
        public Client(int port)
        {
            _tcpClient = new TcpClient("localhost", port);
            _stream = _tcpClient.GetStream();
        }

        /// <summary>
        /// Creates a client connected to a Netler server on a remote host.
        /// </summary>
        /// <param name="hostname">The hostname or IP address of the server.</param>
        /// <param name="port">The TCP port the server is listening on.</param>
        /// <example>
        /// <code>
        /// using var client = new Client("192.168.1.10", 5544);
        /// </code>
        /// </example>
        public Client(string hostname, int port)
        {
            _tcpClient = new TcpClient(hostname, port);
            _stream = _tcpClient.GetStream();
        }

        /// <summary>
        /// Synchronously invokes a route on the Netler server and returns the raw result.
        /// </summary>
        /// <param name="route">The name of the route to invoke.</param>
        /// <param name="parameters">The parameters to pass to the route handler.</param>
        /// <returns>
        /// The return value of the remote handler, boxed as <see cref="object"/>.
        /// Cast or convert to the expected type (e.g. <c>Convert.ToInt32(result)</c>).
        /// </returns>
        /// <example>
        /// <code>
        /// var result = client.Invoke("Add", new object[] { 2, 3 });
        /// var sum = Convert.ToInt32(result); // 5
        /// </code>
        /// </example>
        public object Invoke(string route, object[] parameters)
        {
            var message = new Request(route, parameters);
            var response = default(Response);

            _stream.WriteWithHeader(message.Encode());

            do
            {
                Thread.Sleep(20);
            } while (!_stream.DataAvailable);

            if (_stream.DataAvailable && _stream.CanRead)
            {
                var encodedResponse = _stream.ReadWithHeader();
                response = Response.Decode(encodedResponse);

                if (response.Status == Response.Code.Error)
                {
                    throw new RemoteInvokationFailed($"Method at route {route} threw an error: {response.Data}");
                }
            }

            return response.Data;
        }

        /// <summary>
        /// Asynchronously invokes a route on the Netler server and returns the raw result.
        /// </summary>
        /// <param name="route">The name of the route to invoke.</param>
        /// <param name="parameters">The parameters to pass to the route handler.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>
        /// The return value of the remote handler, boxed as <see cref="object"/>.
        /// Use <see cref="InvokeAsync{T}(string, object[], CancellationToken)"/> to get a
        /// strongly-typed result without a manual cast.
        /// </returns>
        /// <exception cref="Exceptions.RemoteInvokationFailed">
        /// Thrown when the route handler throws an unhandled exception on the server.
        /// </exception>
        /// <example>
        /// <code>
        /// var raw = await client.InvokeAsync("Add", new object[] { 2, 3 });
        /// var sum = Convert.ToInt32(raw);
        /// </code>
        /// </example>
        public async Task<object> InvokeAsync(string route, object[] parameters, CancellationToken cancellationToken = default)
        {
            var message = new Request(route, parameters);
            await _stream.WriteWithHeaderAsync(message.Encode(), cancellationToken);
            var encodedResponse = await _stream.ReadWithHeaderAsync(cancellationToken);
            var response = Response.Decode(encodedResponse);

            if (response.Status == Response.Code.Error)
            {
                throw new RemoteInvokationFailed($"Method at route {route} threw an error: {response.Data}");
            }

            return response.Data;
        }

        /// <summary>
        /// Asynchronously invokes a route on the Netler server and deserialises the result to
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The expected return type. Primitive numeric types are coerced automatically (e.g.
        /// a server returning <c>long</c> can be received as <c>int</c>). Complex types must
        /// be annotated with <c>[MessagePackObject]</c> and their properties with <c>[Key(n)]</c>.
        /// </typeparam>
        /// <param name="route">The name of the route to invoke.</param>
        /// <param name="parameters">The parameters to pass to the route handler.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation.</param>
        /// <returns>The strongly-typed return value of the remote handler.</returns>
        /// <exception cref="Exceptions.RemoteInvokationFailed">
        /// Thrown when the route handler throws an unhandled exception on the server.
        /// </exception>
        /// <example>
        /// <code>
        /// // Primitive types
        /// var sum  = await client.InvokeAsync&lt;int&gt;("Add",    new object[] { 2, 3 });
        /// var pong = await client.InvokeAsync&lt;string&gt;("Ping", new object[0]);
        ///
        /// // Complex type — EchoResponse must carry [MessagePackObject]
        /// var reply = await client.InvokeAsync&lt;EchoResponse&gt;("Echo",
        ///     new object[] { new EchoRequest { Text = "hello" } });
        /// </code>
        /// </example>
        public async Task<T> InvokeAsync<T>(string route, object[] parameters, CancellationToken cancellationToken = default)
        {
            var raw = await InvokeAsync(route, parameters, cancellationToken);
            return TypedConvert.To<T>(raw);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _stream.Dispose();
            _tcpClient.Dispose();
        }
    }
}
