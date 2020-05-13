using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// ApiAction的过滤器抽象特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiActionFilterAttribute : Attribute, IApiActionFilterAttribute
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
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        async Task IApiActionFilterAttribute.BeforeRequestAsync(ApiActionContext context)
        {
            if (this.Enable == true)
            {
                await this.BeforeRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        async Task IApiActionFilterAttribute.AfterRequestAsync(ApiActionContext context)
        {
            if (this.Enable == true)
            {
                await this.AfterRequestAsync(context).ConfigureAwait(false);
            }
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
