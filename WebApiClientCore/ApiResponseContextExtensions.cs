using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
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
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public static async Task<object?> JsonDeserializeAsync(this ApiResponseContext context, Type objType)
        {            
            var response = context.HttpContext.ResponseMessage;
            if (response == null)
            {
                return objType.DefaultValue();
            }

            var content = response.Content;
            var options = context.HttpContext.HttpApiOptions.JsonDeserializeOptions;
            return await content.ReadAsJsonAsync(objType, options, context.RequestAborted);
        }
         
        /// <summary>
        /// 使用Xml反序列化响应内容为目标类型
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objType">目标类型</param>
        /// <returns></returns>
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        public static async Task<object?> XmlDeserializeAsync(this ApiResponseContext context, Type objType)
        {
            var response = context.HttpContext.ResponseMessage;
            if (response == null)
            {
                return objType.DefaultValue();
            }

            var content = response.Content;
            var options = context.HttpContext.HttpApiOptions.XmlDeserializeOptions;

#if NET5_0_OR_GREATER
            var xml = await content.ReadAsStringAsync(context.RequestAborted).ConfigureAwait(false);
#else
            var xml = await content.ReadAsStringAsync().ConfigureAwait(false);
#endif
            return XmlSerializer.Deserialize(xml, objType, options);
        }
    }
}
