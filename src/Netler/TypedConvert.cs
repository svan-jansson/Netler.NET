using MessagePack;
using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")]

namespace Netler
{
    internal static class TypedConvert
    {
        internal static T To<T>(object raw)
        {
            if (raw == null) return default;

            // 1. Direct — already the right type (most common for primitives)
            if (raw is T typed) return typed;

            // 2. Primitive coercion (int/long/double mismatches from MessagePack typeless decode)
            if (raw is IConvertible)
            {
                try { return (T)Convert.ChangeType(raw, typeof(T)); }
                catch (InvalidCastException) { }
                catch (FormatException) { }
            }

            // 3. [MessagePackObject] types: re-encode the raw value and decode as T
            //    Raw arrived via typeless deserialiser (e.g. as object[] for array-keyed objects).
            //    MessagePack's standard resolver handles [MessagePackObject] correctly.
            var bytes = MessagePackSerializer.Serialize(raw);
            return MessagePackSerializer.Deserialize<T>(bytes);
        }
    }
}
