using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// 提供URL的编解码功能
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
            var encoded = HttpUtility.UrlEncode(bytes, 0, bytes.Length);
            return Encoding.ASCII.GetString(encoded);
        }

        /// <summary>
        /// Url编码为字节组
        /// </summary>
        /// <param name="bytes">url字节组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return null;
            }

            int cSpaces = 0;
            int cUnsafe = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];

                if (ch == ' ')
                    cSpaces++;
                else if (!HttpUtility.IsUrlSafeChar(ch))
                    cUnsafe++;
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
            byte[] expandedBytes = new byte[count + cUnsafe * 2];
            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];
                char ch = (char)b;

                if (HttpUtility.IsUrlSafeChar(ch))
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
                    expandedBytes[pos++] = (byte)HttpUtility.IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)HttpUtility.IntToHex(b & 0x0f);
                }
            }

            return expandedBytes;
        }



        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="bytes">字节组</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlDecode(byte[] bytes, Encoding encoding)
        {
            if (bytes == null)
            {
                return null;
            }
            return HttpUtility.UrlDecode(bytes, 0, bytes.Length, encoding);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="bytes">字节组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">数量</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
        {
            UrlDecoder helper = new UrlDecoder(count, encoding);

            // go through the bytes collapsing %XX and %uXXXX and appending
            // each byte as byte, with exception of %uXXXX constructs that
            // are appended as chars

            for (int i = 0; i < count; i++)
            {
                int pos = offset + i;
                byte b = bytes[pos];

                // The code assumes that + and % cannot be in multibyte sequence

                if (b == '+')
                {
                    b = (byte)' ';
                }
                else if (b == '%' && i < count - 2)
                {
                    if (bytes[pos + 1] == 'u' && i < count - 5)
                    {
                        int h1 = HttpUtility.HexToInt((char)bytes[pos + 2]);
                        int h2 = HttpUtility.HexToInt((char)bytes[pos + 3]);
                        int h3 = HttpUtility.HexToInt((char)bytes[pos + 4]);
                        int h4 = HttpUtility.HexToInt((char)bytes[pos + 5]);

                        if (h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0)
                        {   // valid 4 hex chars
                            char ch = (char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4);
                            i += 5;

                            // don't add as byte
                            helper.AddChar(ch);
                            continue;
                        }
                    }
                    else
                    {
                        int h1 = HttpUtility.HexToInt((char)bytes[pos + 1]);
                        int h2 = HttpUtility.HexToInt((char)bytes[pos + 2]);

                        if (h1 >= 0 && h2 >= 0)
                        {     // valid 2 hex chars
                            b = (byte)((h1 << 4) | h2);
                            i += 2;
                        }
                    }
                }

                helper.AddByte(b);
            }
            return helper.ToString();
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlDecode(string str, Encoding encoding)
        {
            if (str == null)
            {
                return null;
            }

            int count = str.Length;
            UrlDecoder helper = new UrlDecoder(count, encoding);

            // go through the string's chars collapsing %XX and %uXXXX and
            // appending each char as char, with exception of %XX constructs
            // that are appended as bytes

            for (int pos = 0; pos < count; pos++)
            {
                char ch = str[pos];

                if (ch == '+')
                {
                    ch = ' ';
                }
                else if (ch == '%' && pos < count - 2)
                {
                    if (str[pos + 1] == 'u' && pos < count - 5)
                    {
                        int h1 = HttpUtility.HexToInt(str[pos + 2]);
                        int h2 = HttpUtility.HexToInt(str[pos + 3]);
                        int h3 = HttpUtility.HexToInt(str[pos + 4]);
                        int h4 = HttpUtility.HexToInt(str[pos + 5]);

                        if (h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0)
                        {   // valid 4 hex chars
                            ch = (char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4);
                            pos += 5;

                            // only add as char
                            helper.AddChar(ch);
                            continue;
                        }
                    }
                    else
                    {
                        int h1 = HttpUtility.HexToInt(str[pos + 1]);
                        int h2 = HttpUtility.HexToInt(str[pos + 2]);

                        if (h1 >= 0 && h2 >= 0)
                        {     // valid 2 hex chars
                            byte b = (byte)((h1 << 4) | h2);
                            pos += 2;

                            // don't add as char
                            helper.AddByte(b);
                            continue;
                        }
                    }
                }

                if ((ch & 0xFF80) == 0)
                    helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
                else
                    helper.AddChar(ch);
            }

            return helper.ToString();
        }

        /// <summary>
        /// hex转为int
        /// </summary>
        /// <param name="h">hex</param>
        /// <returns></returns>
        private static int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' :
                (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
                (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
                -1;
        }

        /// <summary>
        /// int转为hex
        /// </summary>
        /// <param name="n">int</param>
        /// <returns></returns>
        private static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + (int)'0');
            }
            else
            {
                return (char)(n - 10 + (int)'a');
            }
        }

        /// <summary>
        /// 获取字符是否为url安全字符
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




        private class UrlDecoder
        {
            private int _bufferSize;

            // Accumulate characters in a special array
            private int _numChars;
            private char[] _charBuffer;

            // Accumulate bytes for decoding into characters in a special array
            private int _numBytes;
            private byte[] _byteBuffer;

            // Encoding to convert chars to bytes
            private Encoding _encoding;

            private void FlushBytes()
            {
                if (_numBytes > 0)
                {
                    _numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
                    _numBytes = 0;
                }
            }

            public UrlDecoder(int bufferSize, Encoding encoding)
            {
                _bufferSize = bufferSize;
                _encoding = encoding;

                _charBuffer = new char[bufferSize];
                // byte buffer created on demand
            }

            public void AddChar(char ch)
            {
                if (_numBytes > 0)
                    FlushBytes();

                _charBuffer[_numChars++] = ch;
            }

            public void AddByte(byte b)
            {
                // if there are no pending bytes treat 7 bit bytes as characters
                // this optimization is temp disable as it doesn't work for some encodings
                /*
                                if (_numBytes == 0 && ((b & 0x80) == 0)) {
                                    AddChar((char)b);
                                }
                                else
                */
                {
                    if (_byteBuffer == null)
                        _byteBuffer = new byte[_bufferSize];

                    _byteBuffer[_numBytes++] = b;
                }
            }

            public override string ToString()
            {
                if (_numBytes > 0)
                {
                    FlushBytes();
                }

                if (_numChars > 0)
                {
                    return new String(_charBuffer, 0, _numChars);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}
