using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数值作为请求头   
    /// 对于复杂类型的参数值，将拆解作为多个Header
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class HeadersAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var options = context.HttpApiConfig.FormatOptions;
            var formatter = context.HttpApiConfig.KeyValueFormatter;
            var keyValues = formatter.Serialize(parameter, options);

            foreach (var kv in keyValues)
            {
                var name = kv.Key.Replace("_", "-");
                var header = new HeaderAttribute(name, kv.Value);
                await header.BeforeRequestAsync(context);
            }
        }
    }
}
