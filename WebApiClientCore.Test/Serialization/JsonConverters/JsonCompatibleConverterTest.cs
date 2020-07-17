using System;
using System.Text.Json;
using WebApiClientCore.Serialization.JsonConverters;
using Xunit;

namespace WebApiClientCore.Test.Serialization.JsonConverters
{
    public class JsonCompatibleConverterTest
    {
        [Fact]
        public void EnumReaderTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(JsonCompatibleConverter.EnumReader);
            var json = "\"blue\"";
            var color = JsonSerializer.Deserialize<ConsoleColor>(json, options);
            Assert.Equal(ConsoleColor.Blue, color);

            json = "\"Blue\"";
            color = JsonSerializer.Deserialize<ConsoleColor>(json, options);
            Assert.Equal(ConsoleColor.Blue, color);

            var blue = ((int)ConsoleColor.Blue).ToString();
            color = JsonSerializer.Deserialize<ConsoleColor>(blue, options);
            Assert.Equal(ConsoleColor.Blue, color);
        }

        [Fact]
        public void DateTimeReaderTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(JsonCompatibleConverter.DateTimeReader);

            var json = "\"2010-10-10 10:10\"";
            var dateTime = JsonSerializer.Deserialize<DateTime>(json, options);
            Assert.Equal(DateTime.Parse("2010-10-10 10:10"), dateTime);
        }

        [Fact]
        public void DateTimeOffsetReaderTest()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(JsonCompatibleConverter.DateTimeOffsetReader);

            var json = "\"2010-10-10 10:10\"";
            var dateTime = JsonSerializer.Deserialize<DateTimeOffset>(json, options);
            Assert.Equal(DateTimeOffset.Parse("2010-10-10 10:10"), dateTime);
        }
    }
}
