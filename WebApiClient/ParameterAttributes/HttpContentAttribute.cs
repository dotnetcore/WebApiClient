
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
    /// 表示参数为HttpContent或派生类型的特性    
    /// 此类型的参数可以不用注明HttpContentAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class HttpContentAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
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
        protected virtual void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.RequestMessage.Content = parameter.Value as HttpContent;
        }
    }
}
