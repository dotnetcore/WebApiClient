using System;
using System.Buffers;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供http编码
    /// </summary>
    static class HttpEncoder
    {
        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> UrlEncode(ReadOnlySpan<byte> span)
        {
            if (IsNeedUrlEncode(span, out var cUnsafe) == false)
            {
                return span;
            }

            var pos = 0;
            var length = span.Length + cUnsafe * 2;
            var expandedSpan = new byte[length].AsSpan();
            for (var i = 0; i < span.Length; i++)
            {
                var b = span[i];
                var ch = (char)b;

                if (IsUrlSafeChar(ch))
                {
                    expandedSpan[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedSpan[pos++] = (byte)'+';
                }
                else
                {
                    expandedSpan[pos++] = (byte)'%';
                    expandedSpan[pos++] = (byte)ToCharLower(b >> 4);
                    expandedSpan[pos++] = (byte)ToCharLower(b);
                }
            }

            return expandedSpan;
        }

        /// <summary>
        /// Uri编码
        /// </summary>
        /// <param name="span"></param>
        /// <param name="bufferWriter"></param>
        public static void UrlEncode(ReadOnlySpan<byte> span, IBufferWriter<byte> bufferWriter)
        {
            if (IsNeedUrlEncode(span, out var cUnsafe) == false)
            {
                span.CopyTo(bufferWriter.GetSpan(span.Length));
                bufferWriter.Advance(span.Length);
                return;
            }

            var pos = 0;
            var length = span.Length + cUnsafe * 2;
            var expandedSpan = bufferWriter.GetSpan(length);

            for (var i = 0; i < span.Length; i++)
            {
                var b = span[i];
                var ch = (char)b;

                if (IsUrlSafeChar(ch))
                {
                    expandedSpan[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedSpan[pos++] = (byte)'+';
                }
                else
                {
                    expandedSpan[pos++] = (byte)'%';
                    expandedSpan[pos++] = (byte)ToCharLower(b >> 4);
                    expandedSpan[pos++] = (byte)ToCharLower(b);
                }
            }

            bufferWriter.Advance(length);
        }

        /// <summary>
        /// 返回是否需要uri编码
        /// </summary>
        /// <param name="span"></param>
        /// <param name="unsafeCharCount"></param>
        /// <returns></returns>
        private static bool IsNeedUrlEncode(ReadOnlySpan<byte> span, out int unsafeCharCount)
        {
            unsafeCharCount = 0;
            if (span.IsEmpty == true)
            {
                return false;
            }

            var spaceCount = 0;
            for (var i = 0; i < span.Length; i++)
            {
                var ch = (char)span[i];
                if (ch == ' ')
                {
                    spaceCount++;
                }
                else if (!IsUrlSafeChar(ch))
                {
                    unsafeCharCount++;
                }
            }

            return !(spaceCount == 0 && unsafeCharCount == 0);
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
