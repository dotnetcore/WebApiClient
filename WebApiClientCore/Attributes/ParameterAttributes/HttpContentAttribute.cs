using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数值理解为HttpContent类型的特性
    /// 例如StringContent、ByteArrayContent、FormUrlEncodedContent等类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class HttpContentAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        async Task IApiParameterAttribute.BeforeRequestAsync(ApiParameterContext context)
        {
            var method = context.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                var message = $"由于使用{method}的请求方法，{context.Parameter.Member}不支持设置为Content";
                throw new HttpApiInvalidOperationException(message);
            }
            await this.SetHttpContentAsync(context).ConfigureAwait(false);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        /// <returns></returns>
        protected virtual Task SetHttpContentAsync(ApiParameterContext context)
        {
            this.SetHttpContent(context);
            return Task.CompletedTask;
        }


        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <exception cref="HttpApiInvalidOperationException"></exception>
        protected virtual void SetHttpContent(ApiParameterContext context)
        {
            if (context.RequestMessage.Content != null)
            {
                var message = $"参数{context.Parameter.Member}必须置前";
                throw new HttpApiInvalidOperationException(message);
            }

            if (context.ParameterValue != null)
            {
                if (context.ParameterValue is HttpContent httpContent)
                {
                    context.RequestMessage.Content = httpContent;
                }
                else
                {
                    var message = $"参数{context.Parameter.Member}必须为HttpContent类型";
                    throw new HttpApiInvalidOperationException(message);
                }
            }
        }
    }
}
