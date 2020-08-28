using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Exceptions;

namespace System.Net.Http
{
    /// <summary>
    /// HttpContent扩展
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// IsBuffered字段
        /// </summary>
        private static readonly Func<HttpContent, bool>? isBuffered;

        /// <summary>
        /// 静态构造器
        /// </summary>
        static HttpContentExtensions()
        {
            var property = typeof(HttpContent).GetProperty("IsBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                isBuffered = Lambda.CreateGetFunc<HttpContent, bool>(property);
            }
        }

        /// <summary>
        /// 获取是否已缓存数据 
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static bool? IsBuffered(this HttpContent httpContent)
        {
            return isBuffered?.Invoke(httpContent);
        }

        /// <summary>
        /// 确保httpContent的内容未被缓存
        /// 已被缓存则抛出HttpContentBufferedException
        /// </summary>
        /// <param name="httpContent"></param>
        /// <exception cref="HttpContentBufferedException"></exception>
        public static void EnsureNotBuffered(this HttpContent httpContent)
        {
            if (httpContent.IsBuffered() == true)
            {
                throw new HttpContentBufferedException();
            }
        }

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
            if (string.IsNullOrEmpty(charSet) == true)
            {
                return Encoding.UTF8;
            }

            var span = charSet.AsSpan().TrimStart('"').TrimEnd('"');
            if (span.Equals(Encoding.UTF8.WebName, StringComparison.OrdinalIgnoreCase))
            {
                return Encoding.UTF8;
            }

            return Encoding.GetEncoding(span.ToString());
        }
    }
}
