
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示参数值理解为HttpContent类型的特性    
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
            await this.SetHttpContentAsync(context, parameter);
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
                var message = $"参数{parameter.ParameterType.Name} {parameter.Name}必须置前";
                throw new HttpApiConfigException(message);
            }
            context.RequestMessage.Content = parameter.Value as HttpContent;
        }
    }
}
