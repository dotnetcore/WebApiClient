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
        /// 获取或设置过滤器的执行排序索引
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
        public async Task BeforeRequestAsync(ApiActionContext context, Func<Task> next)
        {
            if (this.Enable == true)
            {
                await this.BeforeRequestAsync(context).ConfigureAwait(false);
            }
            await next();
        }

        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">下一个执行委托</param>
        /// <returns></returns>
        public async Task AfterRequestAsync(ApiActionContext context, Func<Task> next)
        {
            if (this.Enable == true)
            {
                await this.AfterRequestAsync(context).ConfigureAwait(false);
            }
            await next();
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task BeforeRequestAsync(ApiActionContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task AfterRequestAsync(ApiActionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
