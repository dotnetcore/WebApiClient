using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

            var byteCount = encoding.GetByteCount(str);
            var source = str.Length > 1024 ? new byte[byteCount] : stackalloc byte[byteCount];
            encoding.GetBytes(str, source);

            var destLength = 0;
            if (UrlEncodeTest(source, ref destLength) == false)
            {
                return str;
            }

            var destination = destLength > 1024 ? new byte[destLength] : stackalloc byte[destLength];
            UrlEncodeCore(source, destination);
            return Encoding.ASCII.GetString(destination);
        }

        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="bufferWriter"></param>
        public static void UrlEncode(ReadOnlySpan<char> chars, IBufferWriter<byte> bufferWriter)
        {
            if (chars.IsEmpty)
            {
                return;
            }

            var byteCount = Encoding.UTF8.GetByteCount(chars);
            var source = chars.Length > 1024 ? new byte[byteCount] : stackalloc byte[byteCount];
            Encoding.UTF8.GetBytes(chars, source);

            var destLength = 0;
            if (UrlEncodeTest(source, ref destLength) == false)
            {
                bufferWriter.Write(source);
            }
            else
            {
                var destination = bufferWriter.GetSpan(destLength);
                UrlEncodeCore(source, destination);
                bufferWriter.Advance(destLength);
            }
        }


        /// <summary>
        /// 测试是否需要进行编码
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="destLength">编码后的长度</param> 
        private static bool UrlEncodeTest(ReadOnlySpan<byte> source, ref int destLength)
        {
            destLength = 0;
            if (source.IsEmpty == true)
            {
                return false;
            }

            var cUnsafe = 0;
            var hasSapce = false;
            for (var i = 0; i < source.Length; i++)
            {
                var ch = (char)source[i];
                if (ch == ' ')
                {
                    hasSapce = true;
                }
                else if (!IsUrlSafeChar(ch))
                {
                    cUnsafe++;
                }
            }

            destLength = source.Length + cUnsafe * 2;
            return !(hasSapce == false && cUnsafe == 0);
        }


        /// <summary>
        /// 将source编码到destination
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="destination">目标</param>  
        private static void UrlEncodeCore(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            var index = 0;
            for (var i = 0; i < source.Length; i++)
            {
                var b = source[i];
                var ch = (char)b;

                if (IsUrlSafeChar(ch))
                {
                    destination[index++] = b;
                }
                else if (ch == ' ')
                {
                    destination[index++] = (byte)'+';
                }
                else
                {
                    destination[index++] = (byte)'%';
                    destination[index++] = (byte)ToCharLower(b >> 4);
                    destination[index++] = (byte)ToCharLower(b);
                }
            }
        }

        /// <summary>
        /// 是否为uri安全字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
