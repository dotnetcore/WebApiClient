using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// 提供URL的编码功能
    /// </summary>
    static class HttpUtility
    {
        /// <summary>
        /// 解析cookie
        /// </summary>
        /// <param name="cookieValues">cookie值</param>
        /// <param name="useUrlEncode">是否对cookie的Value进行url utf-8编码</param>
        /// <exception cref="CookieException"></exception>
        /// <returns></returns>
        public static IEnumerable<Cookie> ParseCookie(string cookieValues, bool useUrlEncode)
        {
            if (string.IsNullOrEmpty(cookieValues) == true)
            {
                yield break;
            }

            const string separator = "; ";
            var cookieItems = cookieValues.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in cookieItems)
            {
                var index = item.IndexOf('=');
                if (index <= 0)
                {
                    continue;
                }
                var name = item.Substring(0, index).Trim();
                if (string.IsNullOrEmpty(name) == false)
                {
                    var value = item.Substring(index + 1).Trim();
                    var encoded = useUrlEncode ? UrlEncode(value, Encoding.UTF8) : value;
                    yield return new Cookie(name, encoded);
                }
            }
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlEncode(string str, Encoding encoding)
        {
            if (str == null)
            {
                return null;
            }

            var bytes = encoding.GetBytes(str);
            var encoded = UrlEncode(bytes, 0, bytes.Length);
            return Encoding.ASCII.GetString(encoded);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="bytes">字节组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            var encoded = UrlEncodeCore(bytes, offset, count);
            if (encoded != null && encoded == bytes)
            {
                return (byte[])encoded.Clone();
            }
            else
            {
                return encoded;
            }
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="bytes">字节组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        private static byte[] UrlEncodeCore(byte[] bytes, int offset, int count)
        {
            if (ValidateUrlEncodingParameters(bytes, offset, count) == false)
            {
                return null;
            }

            var cSpaces = 0;
            var cUnsafe = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                var ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    cSpaces++;
                }
                else if (IsUrlSafeChar(ch) == false)
                {
                    cUnsafe++;
                }
            }

            // nothing to expand?
            if (cSpaces == 0 && cUnsafe == 0)
            {
                // DevDiv 912606: respect "offset" and "count"
                if (0 == offset && bytes.Length == count)
                {
                    return bytes;
                }
                else
                {
                    var subarray = new byte[count];
                    Buffer.BlockCopy(bytes, offset, subarray, 0, count);
                    return subarray;
                }
            }

            // expand not 'safe' characters into %XX, spaces to +s
            var expandedBytes = new byte[count + cUnsafe * 2];
            var pos = 0;

            for (int i = 0; i < count; i++)
            {
                var b = bytes[offset + i];
                var ch = (char)b;

                if (IsUrlSafeChar(ch))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
            }

            return expandedBytes;
        }

        /// <summary>
        /// 验证要编码的参数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
        {
            if (bytes == null && count == 0)
            {
                return false;
            }

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (offset < 0 || offset > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0 || offset + count > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return true;
        }


        /// <summary>
        /// int转换为16进制
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static char IntToHex(int n)
        {
            return n <= 9 ? (char)(n + '0') : (char)(n - 10 + 'a');
        }

        /// <summary>
        /// 返回是否为URL安全字符
        /// </summary>
        /// <param name="ch">字符</param>
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
    }
}
