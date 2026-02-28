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
            => p => fn(
                TypedConvert.To<T1>(p[0]));

        /// <summary>Wraps a two-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, TResult>(Func<T1, T2, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]));

        /// <summary>Wraps a three-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]));

        /// <summary>Wraps a four-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]));

        /// <summary>Wraps a five-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]));

        /// <summary>Wraps a six-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]));

        /// <summary>Wraps a seven-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]),
                TypedConvert.To<T7>(p[6]));

        /// <summary>Wraps an eight-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]),
                TypedConvert.To<T7>(p[6]), TypedConvert.To<T8>(p[7]));

        /// <summary>Wraps a nine-parameter function.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> fn)
            => p => fn(
                TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]),
                TypedConvert.To<T7>(p[6]), TypedConvert.To<T8>(p[7]), TypedConvert.To<T9>(p[8]));

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
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3])); return null; };

        /// <summary>Wraps a five-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4])); return null; };

        /// <summary>Wraps a six-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5])); return null; };

        /// <summary>Wraps a seven-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]),
                TypedConvert.To<T7>(p[6])); return null; };

        /// <summary>Wraps an eight-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]),
                TypedConvert.To<T7>(p[6]), TypedConvert.To<T8>(p[7])); return null; };

        /// <summary>Wraps a nine-parameter void action.</summary>
        public static Func<object[], object> Decode<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> fn)
            => p => { fn(TypedConvert.To<T1>(p[0]), TypedConvert.To<T2>(p[1]), TypedConvert.To<T3>(p[2]),
                TypedConvert.To<T4>(p[3]), TypedConvert.To<T5>(p[4]), TypedConvert.To<T6>(p[5]),
                TypedConvert.To<T7>(p[6]), TypedConvert.To<T8>(p[7]), TypedConvert.To<T9>(p[8])); return null; };
    }
}
