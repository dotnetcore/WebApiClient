
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
        /// <exception cref="ApiConfigException"></exception>
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
        /// <exception cref="ApiConfigException"></exception>
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
        /// <exception cref="ApiConfigException"></exception>
        protected virtual void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (context.RequestMessage.Content != null)
            {
                var message = string.Format("参数{0} {1}必须置前", parameter.ParameterType.Name, parameter.Name);
                throw new ApiConfigException(message);
            }
            context.RequestMessage.Content = parameter.Value as HttpContent;
        }
    }
}
