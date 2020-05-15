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
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static KeyValue[] SerializeToKeyValues(this ApiParameterContext context)
        {
            return context.RequestServices
                .GetRequiredService<IKeyValueFormatter>()
                .Serialize(context.Parameter.Name, context.ParameterValue, context.ApiActionContext.ApiOptions.KeyValueSerializeOptions);
        }

        /// <summary>
        /// 序列化对象为Xml
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string SerializeToXml(this ApiParameterContext context, Encoding encoding)
        {
            return context.RequestServices
                .GetRequiredService<IXmlFormatter>()
                .Serialize(context.ParameterValue, encoding);
        }

        /// <summary>
        /// 序列化对象为Json
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static byte[] SerializeToJson(this ApiParameterContext context)
        {
            return context.RequestServices
                .GetRequiredService<IJsonFormatter>()
                .Serialize(context.ParameterValue, context.ApiActionContext.ApiOptions.KeyValueSerializeOptions);
        }
    }
}
