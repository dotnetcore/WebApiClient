using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
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
        [return: NotNullIfNotNull("str")]
        public static string? UrlEncode(string? str, Encoding encoding)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var source = encoding.GetBytes(str).AsSpan();
            using var bufferWriter = new BufferWriter<byte>();
            if (UrlEncodeToBuffer(source, bufferWriter) == false)
            {
                return str;
            }

            var span = bufferWriter.GetWrittenSpan();
            return Encoding.ASCII.GetString(span);
        }

        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="bufferWriter"></param>
        public static void UrlEncode(string? str, IBufferWriter<byte> bufferWriter)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            var source = Encoding.UTF8.GetBytes(str).AsSpan();
            if (UrlEncodeToBuffer(source, bufferWriter) == true)
            {
                return;
            }

            var length = source.Length;
            source.CopyTo(bufferWriter.GetSpan(length));
            bufferWriter.Advance(length);
        }

        /// <summary>
        /// Uri编码到指定bufferWriter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bufferWriter"></param>
        /// <returns>不需要编码则返回false</returns>
        private static bool UrlEncodeToBuffer(ReadOnlySpan<byte> source, IBufferWriter<byte> bufferWriter)
        {
            if (source.IsEmpty == true)
            {
                return false;
            }

            var cUnsafe = 0;
            var cSpace = 0;
            for (var i = 0; i < source.Length; i++)
            {
                var ch = (char)source[i];
                if (ch == ' ')
                {
                    cSpace++;
                }
                else if (!IsUrlSafeChar(ch))
                {
                    cUnsafe++;
                }
            }

            if (cSpace == 0 && cUnsafe == 0)
            {
                return false;
            }

            var index = 0;
            var length = source.Length + cUnsafe * 2;
            var span = bufferWriter.GetSpan(length);

            for (var i = 0; i < source.Length; i++)
            {
                var b = source[i];
                var ch = (char)b;

                if (IsUrlSafeChar(ch))
                {
                    span[index++] = b;
                }
                else if (ch == ' ')
                {
                    span[index++] = (byte)'+';
                }
                else
                {
                    span[index++] = (byte)'%';
                    span[index++] = (byte)ToCharLower(b >> 4);
                    span[index++] = (byte)ToCharLower(b);
                }
            }

            bufferWriter.Advance(length);
            return true;
        }

        /// <summary>
        /// 是否为uri安全字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static bool IsUrlSafeChar(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 转换为小写
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static char ToCharLower(int n)
        {
            n &= 0xF;
            if (n > 9)
            {
                return (char)(n - 10 + 97);
            }
            return (char)(n + 48);
        }
    }
}
