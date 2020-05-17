using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace WebApiClientCore
{
    /// <summary>
    /// ApiParameterContext序列化扩展
    /// </summary>
    static class ApiParameterContextExtensions
    {
        /// <summary>
        /// 序列化参数值为Json
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static byte[] SerializeParameterToJson(this ApiParameterContext context)
        {
            return context.HttpContext.Services
                .GetRequiredService<IJsonFormatter>()
                .Serialize(context.ParameterValue, context.HttpContext.Options.JsonSerializeOptions);
        }

        /// <summary>
        /// 序列化参数值为Xml
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SerializeParameterToXml(this ApiParameterContext context, Encoding encoding)
        {
            return context.HttpContext.Services
                .GetRequiredService<IXmlFormatter>()
                .Serialize(context.ParameterValue, encoding);
        }

        /// <summary>
        /// 序列化参数值为键值对
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static KeyValue[] SerializeParameterToKeyValues(this ApiParameterContext context)
        {
            return context.HttpContext.Services
                .GetRequiredService<IKeyValueFormatter>()
                .Serialize(context.Parameter.Name, context.ParameterValue, context.HttpContext.Options.KeyValueSerializeOptions);
        }
    }
}
