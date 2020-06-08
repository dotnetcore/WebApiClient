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
            var encoding = httpContent.GetEncoding();
            var byteArray = await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);

            return encoding.Equals(dstEncoding)
                ? byteArray
                : Encoding.Convert(encoding, dstEncoding, byteArray);
        }

        /// <summary>
        /// 获取编码信息
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(this HttpContent httpContent)
        {
            var charSet = httpContent.Headers.ContentType?.CharSet;
            if (string.IsNullOrEmpty(charSet) || charSet == Encoding.UTF8.WebName)
            {
                return Encoding.UTF8;
            }
            return Encoding.GetEncoding(charSet);
        }
    }
}
