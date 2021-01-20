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
        /// <returns></returns>
        public abstract Task OnRequestAsync(ApiRequestContext context);


        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task OnResponseAsync(ApiResponseContext context);
    }
}
