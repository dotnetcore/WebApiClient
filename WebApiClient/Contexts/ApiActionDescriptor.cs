using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示请求Api描述
    /// </summary>
    public class ApiActionDescriptor : ICloneable
    {
        /// <summary>
        /// 获取Api名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public ApiActionAttribute[] Attributes { get; internal set; }

        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public ApiParameterDescriptor[] Parameters { get; internal set; }

        /// <summary>
        /// 获取Api返回的TaskOf(T)类型
        /// </summary>
        public Type ReturnTaskType { get; internal set; }

        /// <summary>
        /// 获取Api返回的TaskOf(T)的T类型
        /// </summary>
        public Type ReturnDataType { get; internal set; }

        /// <summary>
        /// 异步执行Api
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task ExecuteAsync(ApiActionContext context)
        {
            foreach (var methodAttribute in context.ApiActionDescriptor.Attributes)
            {
                methodAttribute.BeforeRequest(context);
            }

            foreach (var parameter in context.ApiActionDescriptor.Parameters)
            {
                foreach (var parameterAttribute in parameter.Attributes)
                {
                    parameterAttribute.BeforeRequest(context, parameter);
                }
            }

            context.ResponseMessage = context.HttpApiClient.HttpClient.SendAsync(context.RequestMessage);
            var taskResult = context.ApiReturnAttribute.GetResultAsync(context);
            return TaskConvter.Cast(taskResult, this.ReturnDataType);
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
                ReturnDataType = this.ReturnDataType,
                ReturnTaskType = this.ReturnTaskType,                
                Parameters = this.Parameters.Select(item => (ApiParameterDescriptor)item.Clone()).ToArray()
            };
        }
    }
}
