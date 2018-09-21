using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
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
        /// 获取顺序排序索引
        /// </summary>
        public virtual int OrderIndex { get; private set; }

        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        public bool AllowMultiple
        {
            get
            {
                return this.GetType().IsAllowMultiple();
            }
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        async Task IApiActionFilter.OnBeginRequestAsync(ApiActionContext context)
        {
            if (this.Enable == true)
            {
                await this.OnBeginRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        async Task IApiActionFilter.OnEndRequestAsync(ApiActionContext context)
        {
            if (this.Enable == true)
            {
                await this.OnEndRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnBeginRequestAsync(ApiActionContext context)
        {
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual Task OnEndRequestAsync(ApiActionContext context)
        {
            return ApiTask.CompletedTask;
        }
    }
}
