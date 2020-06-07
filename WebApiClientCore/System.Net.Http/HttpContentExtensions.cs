using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// HttpContent扩展
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// 读取为二进制数组并转换为utf8编码
        /// </summary>
        /// <param name="httpContent"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Task<byte[]> ReadAsUtf8ByteArrayAsync(this HttpContent httpContent)
        {
            return httpContent.ReadAsByteArrayAsync(Encoding.UTF8);
        }

        /// <summary>
        /// 读取为二进制数组并转换为指定的编码
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="dstEncoding">目标编码</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static async Task<byte[]> ReadAsByteArrayAsync(this HttpContent httpContent, Encoding dstEncoding)
        {
            var byteArray = await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
            var charSet = httpContent.Headers.ContentType?.CharSet;
            var encoding = string.IsNullOrEmpty(charSet) ? Encoding.UTF8 : Encoding.GetEncoding(charSet);

            return encoding.Equals(dstEncoding)
                ? byteArray
                : Encoding.Convert(encoding, dstEncoding, byteArray);
        }
    }
}
