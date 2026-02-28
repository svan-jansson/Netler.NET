using MessagePack;
using System;
using System.Buffers;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")]

namespace Netler
{
    internal static class TypedConvert
    {
        // Entry point: delegate immediately to the per-T cached converter.
        internal static T To<T>(object raw) => Converter<T>.Invoke(raw);

        // ── Optimization 1 & 2: static generic converter cache ───────────────
        // Build() runs exactly once per closed type T (at first use). The
        // resulting delegate is stored and reused on every subsequent call,
        // eliminating the per-call if-chain, IConvertible vtable check, and
        // Convert.ChangeType overhead.
        //
        // Each inline numeric branch replaces the general IConvertible path
        // with a direct Convert.ToXxx call — no interface dispatch, no try/catch,
        // no intermediate boxed object from ChangeType.
        private static class Converter<T>
        {
            internal static readonly Func<object, T> Invoke = Build();

            private static Func<object, T> Build()
            {
                var t = typeof(T);

                if (t == typeof(int))     return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToInt32(raw);
                if (t == typeof(long))    return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToInt64(raw);
                if (t == typeof(double))  return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToDouble(raw);
                if (t == typeof(float))   return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToSingle(raw);
                if (t == typeof(decimal)) return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToDecimal(raw);
                if (t == typeof(bool))    return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToBoolean(raw);
                if (t == typeof(byte))    return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToByte(raw);
                if (t == typeof(sbyte))   return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToSByte(raw);
                if (t == typeof(short))   return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToInt16(raw);
                if (t == typeof(ushort))  return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToUInt16(raw);
                if (t == typeof(uint))    return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToUInt32(raw);
                if (t == typeof(ulong))   return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToUInt64(raw);
                if (t == typeof(char))    return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToChar(raw);
                if (t == typeof(string))  return raw => raw is T v ? v : raw == null ? default : (T)(object)Convert.ToString(raw);

                // Complex / [MessagePackObject] types: direct cast first, then
                // a pooled-buffer MessagePack round-trip (optimization 3).
                return raw =>
                {
                    if (raw == null) return default;
                    if (raw is T typed) return typed;
                    return RoundTrip<T>(raw);
                };
            }
        }

        // ── Optimization 3: pooled-buffer MessagePack round-trip ─────────────
        // Serialises raw into a buffer rented from ArrayPool<byte> (no LOH
        // pressure, no GC allocation for the wire bytes), then deserialises
        // directly to T. The rented buffer is returned in the finally block.
        private static T RoundTrip<T>(object raw)
        {
            var writer = new PooledBufferWriter();
            try
            {
                MessagePackSerializer.Serialize(writer, raw);
                return MessagePackSerializer.Deserialize<T>(writer.WrittenMemory);
            }
            finally
            {
                writer.Dispose();
            }
        }

        // Minimal IBufferWriter<byte> backed by ArrayPool<byte>.
        // Grows exponentially by doubling when the rented segment is exhausted.
        private sealed class PooledBufferWriter : IBufferWriter<byte>, IDisposable
        {
            private const int InitialSize = 256;
            private byte[] _buffer;
            private int _written;

            internal PooledBufferWriter()
            {
                _buffer = ArrayPool<byte>.Shared.Rent(InitialSize);
                _written = 0;
            }

            internal ReadOnlyMemory<byte> WrittenMemory => new ReadOnlyMemory<byte>(_buffer, 0, _written);

            public void Advance(int count) => _written += count;

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                Grow(sizeHint);
                return new Memory<byte>(_buffer, _written, _buffer.Length - _written);
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                Grow(sizeHint);
                return new Span<byte>(_buffer, _written, _buffer.Length - _written);
            }

            private void Grow(int sizeHint)
            {
                int required = _written + Math.Max(sizeHint, 1);
                if (required <= _buffer.Length) return;

                int newSize = Math.Max(required, _buffer.Length * 2);
                var next = ArrayPool<byte>.Shared.Rent(newSize);
                Buffer.BlockCopy(_buffer, 0, next, 0, _written);
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = next;
            }

            public void Dispose()
            {
                if (_buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(_buffer, clearArray: true);
                    _buffer = null;
                }
            }
        }
    }
}
