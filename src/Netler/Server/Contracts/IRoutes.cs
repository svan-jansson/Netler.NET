using System;

namespace Netler.Contracts
{
    /// <summary>
    /// The route table for a Netler Server. Routes map string names to handler functions.
    /// Use <c>AddTyped</c> (<see cref="TypedRouteExtensions"/>) for strongly-typed registration,
    /// or <see cref="Add"/> with <see cref="Netler.Params.Decode{TResult}(Func{TResult})"/> for
    /// explicit functional composition.
    /// </summary>
    public interface IRoutes
    {
        /// <summary>
        /// Registers a raw route handler. The handler receives the MessagePack-decoded parameter
        /// array and returns an <see cref="object"/> (or <c>null</c> for void handlers).
        /// </summary>
        /// <param name="route">
        /// The name clients use to invoke this handler (e.g. <c>"Add"</c>, <c>"CreateUser"</c>).
        /// </param>
        /// <param name="method">
        /// The handler function. Parameters arrive as a raw <c>object[]</c>; cast or use
        /// <see cref="Netler.Params.Decode{TResult}(Func{TResult})"/> to get typed values.
        /// </param>
        /// <example>
        /// <code>
        /// // Preferred — types are inferred from the lambda
        /// routes.AddTyped("Add", (int a, int b) => a + b);
        ///
        /// // Explicit composition with Params.Decode
        /// routes.Add("Add", Params.Decode((int a, int b) => a + b));
        ///
        /// // Raw — manual decoding
        /// routes.Add("Add", param =>
        /// {
        ///     var a = Convert.ToInt32(param[0]);
        ///     var b = Convert.ToInt32(param[1]);
        ///     return a + b;
        /// });
        /// </code>
        /// </example>
        void Add(string route, Func<object[], object> method);

        /// <summary>
        /// Invokes the handler registered under <paramref name="route"/> with the supplied parameters.
        /// </summary>
        /// <param name="route">The name of the route to invoke.</param>
        /// <param name="parameters">The parameter array to pass to the handler.</param>
        /// <returns>The return value of the handler, or <c>null</c> for void handlers.</returns>
        /// <exception cref="Netler.Exceptions.RouteUndefined">
        /// Thrown when no handler is registered under <paramref name="route"/>.
        /// </exception>
        /// <exception cref="Netler.Exceptions.RouteMethodCallFailed">
        /// Thrown when the handler throws an unhandled exception.
        /// </exception>
        object Invoke(string route, object[] parameters);
    }
}
