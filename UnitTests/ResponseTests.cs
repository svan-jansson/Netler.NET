using Netler;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{
    public class ResponseTests
    {
        [Theory]
        [InlineData("a string")]
        [InlineData(true)]
        [InlineData(234)]
        [InlineData(555.45)]
        [InlineData(null)]
        public void CanEncodeDecodeResponse(object data)
        {
            var inputType = data == null ? typeof(string) : data.GetType();
            var input = new Response
            {
                Status = Response.Code.Ok,
                Data = data
            };

            var encoded = input.Encode();
            var decoded = Response.Decode(encoded);

            Assert.Equal(input.Status, decoded.Status);
            Assert.Equal(input.Data, Convert.ChangeType(decoded.Data, inputType));
        }

        [Fact]
        public void CanEncodeDecodeDictionaryResponse()
        {
            var input = new Response
            {
                Status = Response.Code.Ok,
                Data = new Dictionary<string, object>
                {
                    { "a key", "a value" },
                    { "another key", false },
                    { "yet another key", null }
                }
            };

            var encoded = input.Encode();
            var decoded = Response.Decode(encoded);

            Assert.Equal(input.Status, decoded.Status);
            Assert.Equal(input.Data, decoded.Data);
        }

        [Fact]
        public void CanEncodeDecodeToAnonymousResponse()
        {
            var input = Response.FromNamed(Response.Code.Ok, new TestClass
            {
                Value1 = "A",
                Value2 = "B"
            });

            var encoded = input.Encode();
            var decoded = Response.Decode(encoded);

            Assert.Equal(input.Status, decoded.Status);
            Assert.Equal(input.Data, decoded.Data as Dictionary<object, object>);
        }

        private class TestClass
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }
    }
}
