using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Internals;

namespace System.Net.Http
{
    /// <summary>
    /// HttpContent扩展
    /// </summary>
    public static class HttpContentExtensions
    {
        private const string IsBufferedPropertyName = "IsBuffered";
        private const string IsBufferedGetMethodName = "get_IsBuffered";

        /// <summary>
        /// IsBuffered字段
        /// </summary>
        private static readonly Func<HttpContent, bool>? isBufferedFunc;

        /// <summary>
        /// 静态构造器
        /// </summary>
        static HttpContentExtensions()
        {
            var property = typeof(HttpContent).GetProperty(IsBufferedPropertyName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
#if NET8_0_OR_GREATER
                if (property.GetGetMethod(nonPublic: true)?.Name == IsBufferedGetMethodName)
                {
                    isBufferedFunc = GetIsBuffered;
                }
#endif
                if (isBufferedFunc == null)
                {
                    isBufferedFunc = LambdaUtil.CreateGetFunc<HttpContent, bool>(property);
                }
            }
        }

#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = IsBufferedGetMethodName)]
        private static extern bool GetIsBuffered(HttpContent httpContent);
#endif

        /// <summary>
        /// 获取是否已缓存数据 
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static bool? IsBuffered(this HttpContent httpContent)
        {
            return isBufferedFunc == null ? null : isBufferedFunc(httpContent);
        }

        /// <summary>
        /// 确保HttpContent的内容未被缓存
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
        /// 读取 json 内容为指定的类型
        /// </summary>
        /// <param name="content">http内容</param>
        /// <param name="objType">目标类型</param>
        /// <param name="options">json反序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public static async Task<object?> ReadAsJsonAsync(this HttpContent content, Type objType, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
        {
#if NET5_0_OR_GREATER
            return await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync(content, objType, options, cancellationToken);
#else
            var encoding = content.GetEncoding();
            if (Encoding.UTF8.Equals(encoding) == false)
            {
                var byteArray = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (byteArray.Length == 0)
                {
                    return objType.DefaultValue();
                }
                var utf8Json = Encoding.Convert(encoding, Encoding.UTF8, byteArray);
                return JsonSerializer.Deserialize(utf8Json, objType, options);
            }

            if (content.IsBuffered() == false)
            {
                var utf8Json = await content.ReadAsStreamAsync().ConfigureAwait(false);
                return await JsonSerializer.DeserializeAsync(utf8Json, objType, options).ConfigureAwait(false);
            }
            else
            {
                var utf8Json = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return utf8Json.Length == 0 ? objType.DefaultValue() : JsonSerializer.Deserialize(utf8Json, objType, options);
            }
#endif
        }

        /// <summary>
        /// 读取 json 内容为指定的类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="content">http内容</param>
        /// <param name="options">json反序列化选项</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
        {
#if NET5_0_OR_GREATER
            return await System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync<T>(content, options, cancellationToken);
#else
            var encoding = content.GetEncoding();
            if (Encoding.UTF8.Equals(encoding) == false)
            {
                var byteArray = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (byteArray.Length == 0)
                {
                    return default;
                }
                var utf8Json = Encoding.Convert(encoding, Encoding.UTF8, byteArray);
                return JsonSerializer.Deserialize<T>(utf8Json, options);
            }

            if (content.IsBuffered() == false)
            {
                var utf8Json = await content.ReadAsStreamAsync().ConfigureAwait(false);
                return await JsonSerializer.DeserializeAsync<T>(utf8Json, options).ConfigureAwait(false);
            }
            else
            {
                var utf8Json = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return utf8Json.Length == 0 ? default : JsonSerializer.Deserialize<T>(utf8Json, options);
            }
#endif
        }

        /// <summary>
        /// 读取为二进制数组并转换为 utf8 编码
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
            return encoding.Equals(dstEncoding) ? byteArray : Encoding.Convert(encoding, dstEncoding, byteArray);
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
