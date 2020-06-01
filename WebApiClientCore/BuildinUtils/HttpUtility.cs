using System;
using System.Buffers;
using System.Text;

namespace WebApiClientCore
{
    /// <summary>
    /// http工具类
    /// </summary>
    static class HttpUtility
    {
        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlEncode(string str, Encoding encoding)
        {
            var span = encoding.GetBytes(str).AsSpan();
            var bytes = HttpEncoder.UrlEncode(span);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="bufferWriter"></param>
        public static void UrlEncode(string? str, IBufferWriter<byte> bufferWriter)
        {
            if (str != null)
            {
                var span = Encoding.UTF8.GetBytes(str).AsSpan();
                HttpEncoder.UrlEncode(span, bufferWriter);
            }
        }
    }
}
