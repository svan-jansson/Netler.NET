using System;

namespace Netler
{
    /// <summary>
    /// Functional composition helper that wraps a typed delegate into the
    /// <c>Func&lt;object[], object&gt;</c> signature expected by <c>IRoutes.Add</c>.
    /// Parameter decoding (direct cast → primitive coercion → MessagePack round-trip)
    /// is handled transparently; complex types must carry <c>[MessagePackObject]</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// routes.Add("Add",    Params.Decode&lt;int, int, int&gt;((a, b) => a + b));
    /// routes.Add("Double", Params.Decode&lt;int, int&gt;(x => x * 2));
    /// routes.Add("Ping",   Params.Decode&lt;string&gt;(() => "pong"));
    /// routes.Add("Log",    Params.Decode&lt;string&gt;(msg => { /* void */ }));
    /// routes.Add("Create", Params.Decode&lt;CreateRequest, CreateResponse&gt;(req => new CreateResponse { Id = 1 }));
    /// </code>
    /// </example>
    public static class Params
    {
        // ── Func overloads (with return value) ───────────────────────────────

        /// <summary>Wraps a zero-parameter function.</summary>
        public static Func<object[], object> Decode<TResult>(Func<TResult> fn)
            => _ => fn();

        /// <summary>Wraps a one-parameter function.</summary>
        public static Func<object[], object> Decode<T1, TResult>(Func<T1, TResult> fn)
            => p => fn(TypedConvert.To<T1>(p[0]));

        /// <summary>Wraps a two-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, TResult>(Func<T1, T2, TResult> fn)
            => p => fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]));

        /// <summary>Wraps a three-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> fn)
            => p => fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]));

        /// <summary>Wraps a four-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> fn)
            => p => fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]), TypedConvert.To<T4>(p[3]));

        // ── Action overloads (void, route returns null) ───────────────────────

        /// <summary>Wraps a zero-parameter void action.</summary>
        public static Func<object[], object> Decode(Action fn)
            => _ => { fn(); return null; };

        /// <summary>Wraps a one-parameter void action.</summary>
        public static Func<object[], object> Decode<T1>(Action<T1> fn)
            => p => { fn(TypedConvert.To<T1>(p[0])); return null; };

        /// <summary>Wraps a two-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2>(Action<T1, T2> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1])); return null; };

        /// <summary>Wraps a three-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3>(Action<T1, T2, T3> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2])); return null; };

        /// <summary>Wraps a four-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4>(Action<T1, T2, T3, T4> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]), TypedConvert.To<T4>(p[3])); return null; };
    }
}
