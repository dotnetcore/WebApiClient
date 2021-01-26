using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供Encoding扩展
    /// </summary>
    static class EncodingExtensions
    {
        /// <summary>
        /// 转换编码
        /// </summary>
        /// <param name="srcEncoding"></param>
        /// <param name="dstEncoding">目标编码</param>
        /// <param name="buffer">源内容</param>
        /// <param name="writer">目标写入器</param>
        public static void Convert(this Encoding srcEncoding, Encoding dstEncoding, ReadOnlySpan<byte> buffer, IBufferWriter<byte> writer)
        {
            var decoder = srcEncoding.GetDecoder();
            var charCount = decoder.GetCharCount(buffer, false);
            var charArray = ArrayPool<char>.Shared.Rent(charCount);

            try
            {
                decoder.Convert(buffer, charArray, true, out _, out var charsUsed, out _);
                Debug.Assert(charCount == charsUsed);
                var chars = charArray.AsSpan().Slice(0, charsUsed);

                var encoder = dstEncoding.GetEncoder();
                var byteCount = encoder.GetByteCount(chars, false);
                var bytes = writer.GetSpan(byteCount);

                encoder.Convert(chars, bytes, true, out _, out var byteUsed, out _);
                Debug.Assert(byteCount == byteUsed);
                writer.Advance(byteUsed);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(charArray);
            }
        }
    }
}
