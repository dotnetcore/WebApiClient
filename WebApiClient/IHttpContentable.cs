using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义将参数值转换为HttpContent设置到请求上下文的方法
    /// </summary>
    public interface IHttpContentable
    {
        /// <summary>
        /// 将参数值转换为HttpContent设置到请求上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的属性</param>
        void SetRquestHttpContent(ApiActionContext context, ApiParameterDescriptor parameter);
    }
}
