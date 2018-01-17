using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 回复内容处理特性抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiReturnAttribute : Attribute, IApiReturnAttribute
    {
        /// <summary>
        /// 获取或设置是否确保回复的http状态码是2xx码
        /// </summary>
        public bool EnsureSuccessStatusCode { get; set; }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task<object> IApiReturnAttribute.GetTaskResult(ApiActionContext context)
        {
            if (this.EnsureSuccessStatusCode == true)
            {
                context.ResponseMessage.EnsureSuccessStatusCode();
            }
            return this.GetTaskResult(context);
        }

        /// <summary>
        /// 获取异步结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected abstract Task<object> GetTaskResult(ApiActionContext context);
    }
}
