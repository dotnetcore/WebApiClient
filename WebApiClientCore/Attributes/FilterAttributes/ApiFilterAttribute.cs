using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// ApiAction的过滤器抽象特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiFilterAttribute : Attribute, IApiFilterAttribute
    {
        /// <summary>
        /// 获取或设置是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 获取或设置执行排序索引
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        public bool AllowMultiple => this.GetType().IsAllowMultiple();

        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        public async Task OnRequestAsync(ApiRequestContext context, Func<Task> next)
        {
            if (this.Enable == true)
            {
                await this.OnRequestAsync(context).ConfigureAwait(false);
            }
            await next();
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        public async Task OnResponseAsync(ApiResponseContext context, Func<Task> next)
        {
            if (this.Enable == true)
            {
                await this.OnResponseAsync(context).ConfigureAwait(false);
            }
            await next();
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnRequestAsync(ApiRequestContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnResponseAsync(ApiResponseContext context)
        {
            return Task.CompletedTask;
        }
    }
}
