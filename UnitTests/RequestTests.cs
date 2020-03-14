using Netler;
using System;
using Xunit;

namespace UnitTests
{
    public class RequestTests
    {
        [Fact]
        public void CanEncodeDecodeRequest()
        {
            var input = new Request
            {
                Route = "Add",
                Parameters = new object[] { 2, 3 }
            };

            var encoded = input.Encode();
            var decoded = Request.Decode(encoded);

            Assert.Equal(input.Route, decoded.Route);
            Assert.Equal(input.Parameters[0], Convert.ToInt32(decoded.Parameters[0]));
            Assert.Equal(input.Parameters[1], Convert.ToInt32(decoded.Parameters[1]));
        }
    }
}
