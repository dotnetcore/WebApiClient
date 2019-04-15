using System;
using Xunit;

namespace WebApiClient.Test
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
            var opt = new FormatOptions() { DateTimeFormat = "xyz", IgnoreNullProperty = true, UseCamelCase = true };
            Assert.True(opt.CloneChange(null) == opt);
            Assert.True(opt.CloneChange("xyz") == opt);

            var yyyy = opt.CloneChange("yyyy");
            Assert.True(yyyy != opt);
            Assert.True(yyyy.DateTimeFormat == "yyyy");
            Assert.True(yyyy.IgnoreNullProperty == opt.IgnoreNullProperty);
            Assert.True(yyyy.UseCamelCase == opt.UseCamelCase);
        }
    }
}
