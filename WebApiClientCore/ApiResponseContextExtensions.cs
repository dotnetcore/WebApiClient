using System;
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
        public static async Task<object?> JsonDeserializeAsync(this ApiResponseContext context, Type objType)
        {
            if (context.HttpContext.ResponseMessage == null)
            {
                return objType.DefaultValue();
            }

            var content = context.HttpContext.ResponseMessage.Content;
            if (content == null)
            {
                return objType.DefaultValue();
            }

            var encoding = content.GetEncoding();
            var options = context.HttpContext.HttpApiOptions.JsonDeserializeOptions;

            if (Encoding.UTF8.Equals(encoding) == false)
            {
                var byteArray = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
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
                return JsonSerializer.Deserialize(utf8Json, objType, options);
            }
        }

        /// <summary>
        /// 使用Xml反序列化响应内容为目标类型
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objType">目标类型</param>
        /// <returns></returns>
        public static async Task<object?> XmlDeserializeAsync(this ApiResponseContext context, Type objType)
        {
            if (context.HttpContext.ResponseMessage == null)
            {
                return objType.DefaultValue();
            }

            var options = context.HttpContext.HttpApiOptions.XmlDeserializeOptions;
            var xml = await context.HttpContext.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return XmlSerializer.Deserialize(xml, objType, options);
        }
    }
}
