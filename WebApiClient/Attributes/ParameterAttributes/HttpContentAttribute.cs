using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
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
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        async Task IApiParameterAttribute.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var method = context.RequestMessage.Method;
            if (method == HttpMethod.Get || method == HttpMethod.Head)
            {
                var message = $"由于使用{method}的请求方法，{parameter.Member}不支持设置为Content";
                throw new HttpApiConfigException(message);
            }
            await this.SetHttpContentAsync(context, parameter).ConfigureAwait(false);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        protected virtual Task SetHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            this.SetHttpContent(context, parameter);
            return ApiTask.CompletedTask;
        }


        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        protected virtual void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (context.RequestMessage.Content != null)
            {
                var message = $"参数{parameter.Member}必须置前";
                throw new HttpApiConfigException(message);
            }

            if (parameter.Value != null)
            {
                if (parameter.Value is HttpContent httpContent)
                {
                    context.RequestMessage.Content = httpContent;
                }
                else
                {
                    var message = $"参数{parameter.Member}必须为HttpContent类型";
                    throw new HttpApiConfigException(message);
                }
            }
        }
    }
}
