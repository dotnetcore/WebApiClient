using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiClientCore.Serialization;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供ApiResponseContext的扩展
    /// </summary>
    public static class ApiResponseContextExtensions
    {
        /// <summary>
        /// 使用Json反序列化响应内容为目标类型
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objType">目标类型</param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
        public static async Task<object?> JsonDeserializeAsync(this ApiResponseContext context, Type objType)
        {
            var response = context.HttpContext.ResponseMessage;
            if (response == null)
            {
                return objType.DefaultValue();
            }

            var content = response.Content;
            var encoding = content.GetEncoding();
            var options = context.HttpContext.HttpApiOptions.JsonDeserializeOptions;

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
        }

        /// <summary>
        /// 使用Xml反序列化响应内容为目标类型
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objType">目标类型</param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
#endif
        public static async Task<object?> XmlDeserializeAsync(this ApiResponseContext context, Type objType)
        {
            var response = context.HttpContext.ResponseMessage;
            if (response == null)
            {
                return objType.DefaultValue();
            }

            var content = response.Content;
            var options = context.HttpContext.HttpApiOptions.XmlDeserializeOptions;
            var xml = await content.ReadAsStringAsync().ConfigureAwait(false);
            return XmlSerializer.Deserialize(xml, objType, options);
        }
    }
}
