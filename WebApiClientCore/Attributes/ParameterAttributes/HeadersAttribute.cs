using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值作为请求头   
    /// 对于复杂类型的参数值，将拆解作为多个Header
    /// </summary>
    public class HeadersAttribute : ApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        public sealed override Task BeforeRequestAsync(ApiParameterContext context)
        {
            var keyValues = context.SerializeToKeyValues();
            foreach (var kv in keyValues)
            {
                var name = kv.Key.Replace("_", "-");
                this.SetHeaderValue(context, name, kv.Value);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        private void SetHeaderValue(ApiParameterContext context, string name, string value)
        {
            if (string.Equals(name, "Cookie", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpApiInvalidOperationException(Resx.unsupported_ManaulCookie);
            }

            var headers = context.HttpContext.RequestMessage.Headers;
            headers.Remove(name);
            if (value != null)
            {
                headers.TryAddWithoutValidation(name, value);
            }
        }
    }
}
