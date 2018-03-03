using System;
using Xunit;
using WebApiClient;

namespace WebApiClientTest
{
    public class FormatOptionsTest
    {
        [Fact]
        public void Test_Default_Properties()
        {
            var opt = new FormatOptions();
            Assert.True(opt.UseCamelCase == false);
            Assert.True(opt.DateTimeFormat == DateTimeFormats.LocalDateTimeFormat);
            Assert.Throws<ArgumentNullException>(() => opt.DateTimeFormat = null);
        }

        [Fact]
        public void Test_CamelCase()
        { 
            Assert.True(FormatOptions.CamelCase("WebApiClient") == "webApiClient");
            Assert.True(FormatOptions.CamelCase("AWebApiClient") == "aWebApiClient");
            Assert.True(FormatOptions.CamelCase("aWebApiClient") == "aWebApiClient");
            Assert.True(FormatOptions.CamelCase("ABWebApiClient") == "abWebApiClient");
        }

        [Fact]
        public void Test_Default_CloneChange()
        {
            var opt = new FormatOptions();
            Assert.True(opt.CloneChange(null) == opt);
            Assert.True(opt.CloneChange(DateTimeFormats.LocalDateTimeFormat) == opt);
            Assert.True(opt.CloneChange("yyyy") != opt);
        }
    }
}
