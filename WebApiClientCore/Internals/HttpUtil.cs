using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebApiClientCore.Internals
{
    /// <summary>
    /// http工具类
    /// </summary>
    public static class HttpUtil
    {
        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(str))]
        public static string? UrlEncode(string? str, Encoding encoding)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var byteCount = encoding.GetByteCount(str);
            var source = str.Length > 1024 ? new byte[byteCount] : stackalloc byte[byteCount];
            encoding.GetBytes(str, source);

            var destinationLength = 0;
            if (UrlEncodeTest(source, ref destinationLength) == false)
            {
                return str;
            }

            var destination = destinationLength > 1024 ? new byte[destinationLength] : stackalloc byte[destinationLength];
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

            var destinationLength = 0;
            if (UrlEncodeTest(source, ref destinationLength) == false)
            {
                bufferWriter.Write(source);
            }
            else
            {
                var destination = bufferWriter.GetSpan(destinationLength);
                UrlEncodeCore(source, destination);
                bufferWriter.Advance(destinationLength);
            }
        }


        /// <summary>
        /// 测试是否需要进行编码
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="destinationLength">编码后的长度</param> 
        private static bool UrlEncodeTest(ReadOnlySpan<byte> source, ref int destinationLength)
        {
            destinationLength = 0;
            if (source.IsEmpty == true)
            {
                return false;
            }

            var cUnsafe = 0;
            var hasSpace = false;
            for (var i = 0; i < source.Length; i++)
            {
                var ch = (char)source[i];
                if (ch == ' ')
                {
                    hasSpace = true;
                }
                else if (!IsUrlSafeChar(ch))
                {
                    cUnsafe++;
                }
            }

            destinationLength = source.Length + cUnsafe * 2;
            return !(hasSpace == false && cUnsafe == 0);
        }


        /// <summary>
        /// 将 source 编码到 destination
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
        /// 是否为Uri安全字符
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
            return n > 9 ? (char)(n - 10 + 97) : (char)(n + 48);
        }
    }
}
