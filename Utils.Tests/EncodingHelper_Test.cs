using System;
using Xunit;
using Bergfall.Oculos.Utils;

namespace Utils.Tests
{
    public class EncodingHelper_Test
    {
        [Fact]
        public void Test1()
        {
            int a = 2;
            int b = 5;
            int sum = a + b;
            Assert.Equal(sum.ToString(), "7");
        }

        [Fact]
        [InlineData(-1)]
        public void IntToBin_Test()
        {
            //new object()[50, 200, 255, 256])
        }
    }
}
