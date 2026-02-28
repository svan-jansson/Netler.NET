using MessagePack;
using Netler;
using Xunit;

namespace UnitTests
{
    public class TypedConvertTests
    {
        [Fact]
        public void DirectCast_ReturnsSameValue()
        {
            var result = TypedConvert.To<int>(42);
            Assert.Equal(42, result);
        }

        [Fact]
        public void DirectCast_String_ReturnsSameValue()
        {
            var result = TypedConvert.To<string>("hello");
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Null_ReturnsDefault()
        {
            var intResult = TypedConvert.To<int>(null);
            var stringResult = TypedConvert.To<string>(null);

            Assert.Equal(0, intResult);
            Assert.Null(stringResult);
        }

        [Fact]
        public void LongToInt_Coercion()
        {
            // MessagePack typeless decode can return long for small integers
            long raw = 7L;
            var result = TypedConvert.To<int>(raw);
            Assert.Equal(7, result);
        }

        [Fact]
        public void IntToLong_Coercion()
        {
            int raw = 99;
            var result = TypedConvert.To<long>(raw);
            Assert.Equal(99L, result);
        }

        [Fact]
        public void DoubleToFloat_Coercion()
        {
            double raw = 3.14;
            var result = TypedConvert.To<float>(raw);
            Assert.Equal((float)3.14, result, precision: 5);
        }

        [Fact]
        public void MessagePackObject_RoundTrip()
        {
            // Simulate what arrives after typeless MessagePack deserialization:
            // a [MessagePackObject] type encoded then decoded as object[] (array-keyed).
            var original = new TypedConvertPoint { X = 10, Y = 20 };
            var bytes = MessagePackSerializer.Serialize(original);
            // Typeless decode gives object[] for array-keyed MessagePackObject
            var raw = MessagePackSerializer.Deserialize<object>(bytes);

            var result = TypedConvert.To<TypedConvertPoint>(raw);

            Assert.Equal(10, result.X);
            Assert.Equal(20, result.Y);
        }

        [MessagePackObject]
        public class TypedConvertPoint
        {
            [Key(0)] public int X { get; set; }
            [Key(1)] public int Y { get; set; }
        }
    }
}
