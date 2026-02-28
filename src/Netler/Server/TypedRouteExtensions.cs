using System;

namespace Netler.Contracts
{
    /// <summary>
    /// Typed overloads for <see cref="IRoutes.Add"/> that compose automatically with
    /// <see cref="Netler.Params.Decode"/>. Type parameters are inferred from the handler
    /// lambda; the underlying wire format is unchanged.
    /// </summary>
    /// <example>
    /// <code>
    /// routes.AddTyped("Add",    (int a, int b) => a + b);
    /// routes.AddTyped("Double", (int x) => x * 2);
    /// routes.AddTyped("Ping",   () => "pong");
    /// routes.AddTyped("Log",    (string msg) => { /* void */ });
    /// routes.AddTyped("Create", (CreateRequest req) => new CreateResponse { Id = 1 });
    /// </code>
    /// </example>
    public static class TypedRouteExtensions
    {
        // ── Func overloads (with return value) ───────────────────────────────

        /// <summary>Adds a typed route with a zero-parameter handler.</summary>
        public static void AddTyped<TResult>(this IRoutes routes, string route, Func<TResult> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a one-parameter handler.</summary>
        public static void AddTyped<T1, TResult>(this IRoutes routes, string route, Func<T1, TResult> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a two-parameter handler.</summary>
        public static void AddTyped<T1, T2, TResult>(this IRoutes routes, string route, Func<T1, T2, TResult> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a three-parameter handler.</summary>
        public static void AddTyped<T1, T2, T3, TResult>(this IRoutes routes, string route, Func<T1, T2, T3, TResult> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a four-parameter handler.</summary>
        public static void AddTyped<T1, T2, T3, T4, TResult>(this IRoutes routes, string route, Func<T1, T2, T3, T4, TResult> fn)
            => routes.Add(route, Params.Decode(fn));

        // ── Action overloads (void return) ───────────────────────────────────

        /// <summary>Adds a typed route with a zero-parameter void handler.</summary>
        public static void AddTyped(this IRoutes routes, string route, Action fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a one-parameter void handler.</summary>
        public static void AddTyped<T1>(this IRoutes routes, string route, Action<T1> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a two-parameter void handler.</summary>
        public static void AddTyped<T1, T2>(this IRoutes routes, string route, Action<T1, T2> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a three-parameter void handler.</summary>
        public static void AddTyped<T1, T2, T3>(this IRoutes routes, string route, Action<T1, T2, T3> fn)
            => routes.Add(route, Params.Decode(fn));

        /// <summary>Adds a typed route with a four-parameter void handler.</summary>
        public static void AddTyped<T1, T2, T3, T4>(this IRoutes routes, string route, Action<T1, T2, T3, T4> fn)
            => routes.Add(route, Params.Decode(fn));
    }
}
