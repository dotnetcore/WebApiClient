using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示特殊的结果特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class SpecialReturnAttribute : Attribute, IApiReturnAttribute
    {
        /// <summary>
        /// 获取执行排序索引
        /// </summary>
        public int OrderIndex { get; }

        /// <summary>
        /// 获取或设置是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 获取本类型是否允许在接口与方法上重复
        /// </summary>
        public bool AllowMultiple => this.GetType().IsAllowMultiple();

        /// <summary>
        /// 获取接受的媒体类型
        /// </summary>
        MediaTypeWithQualityHeaderValue? IApiReturnAttribute.AcceptContentType { get; }

        /// <summary>
        /// 特殊的结果特性
        /// </summary>
        public SpecialReturnAttribute()
            : this(1d)
        {
        }

        /// <summary>
        /// 特殊的结果特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public SpecialReturnAttribute(double acceptQuality)
        {
            this.OrderIndex = (int)((1d - acceptQuality) * int.MaxValue);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public Task OnRequestAsync(ApiRequestContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param> 
        /// <returns></returns>
        public Task OnResponseAsync(ApiResponseContext context)
        {
            return this.SetResultAsync(context);
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public abstract Task SetResultAsync(ApiResponseContext context);
    }
}
