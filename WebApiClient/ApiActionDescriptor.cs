using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClient
{
    /// <summary>
    /// 表示请求Api描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class ApiActionDescriptor : ICloneable
    {
        /// <summary>
        /// 获取Api名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public IApiActionAttribute[] Attributes { get; internal set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public IApiActionFilterAttribute[] Filters { get; internal set; }

        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public ApiParameterDescriptor[] Parameters { get; internal set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public ApiReturnDescriptor Return { get; internal set; }

        /// <summary>
        /// 异步执行api
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async Task<object> ExecuteAsync(ApiActionContext context)
        {
            var apiAction = context.ApiActionDescriptor;

            foreach (var actionAttribute in apiAction.Attributes)
            {
                await actionAttribute.BeforeRequestAsync(context);
            }

            foreach (var parameter in apiAction.Parameters)
            {
                foreach (var parameterAttribute in parameter.Attributes)
                {
                    await parameterAttribute.BeforeRequestAsync(context, parameter);
                }
            }

            foreach (var filter in apiAction.Filters)
            {
                await filter.OnBeginRequestAsync(context);
            }

            await this.SendAsync(context);

            foreach (var filter in apiAction.Filters)
            {
                await filter.OnEndRequestAsync(context);
            }

            return await apiAction.Return.Attribute.GetTaskResult(context);
        }

        /// <summary>
        /// 执行发送请求
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private async Task SendAsync(ApiActionContext context)
        {
            var client = context.HttpApiConfig.HttpClient;
            context.ResponseMessage = await client.SendAsync(context.RequestMessage);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ApiActionDescriptor
            {
                Name = this.Name,
                Attributes = this.Attributes,
                Return = this.Return,
                Filters = this.Filters,
                Parameters = this.Parameters.Select(item => (ApiParameterDescriptor)item.Clone()).ToArray()
            };
        }
    }
}
