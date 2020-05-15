using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// ApiAction修饰特性抽象
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ApiActionAttribute : Attribute, IApiActionAttribute
    {
        /// <summary>
        /// 获取顺序排序索引
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
        /// <param name="next">下一个中间件</param>
        /// <returns></returns>
        public async Task OnRequestAsync(ApiRequestContext context, Func<Task> next)
        {
            await this.OnRequestAsync(context);

            // 目前场景无中断需求，无条件执行下个中间件
            await next();
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task OnRequestAsync(ApiRequestContext context);
    }
}
