using System;
using Xunit;

namespace WebApiClientCore.Test.BuildinBuffers
{
    public class BufferWriterTest
    {
        [Fact]
        public void WriteTest()
        {
            using var writer = new BufferWriter<char>(1);
            writer.Write('H');
            Assert.Equal(1, writer.WrittenCount);
            writer.Write('e');
            Assert.Equal(2, writer.WrittenCount);
            writer.Write("llo");
            Assert.True(writer.GetWrittenSpan().SequenceEqual("Hello"));

            writer.Clear();
            Assert.Equal(0, writer.WrittenCount);

            var span = writer.GetSpan();
            "Word".AsSpan().CopyTo(span);
            writer.Advance(4);
            Assert.True(writer.GetWrittenSpan().SequenceEqual("Word"));
        }
    }
}
