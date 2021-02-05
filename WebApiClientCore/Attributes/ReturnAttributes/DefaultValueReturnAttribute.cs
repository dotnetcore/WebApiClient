using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将结果设置为返回类型的默认值抽象特性
    /// </summary> 
    public abstract class DefaultValueReturnAttribute : CustomValueReturnAttribute
    {
        /// <summary>
        /// 将结果设置为返回类型的默认值抽象特性
        /// </summary>
        public DefaultValueReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 将结果设置为返回类型的默认值抽象特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public DefaultValueReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 如果有结果值，则设置结果值到上下文的Result属性
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed override Task SetResultAsync(ApiResponseContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            if (response != null && this.CanUseDefaultValue(response))
            {
                context.Result = context.ActionDescriptor.Return.DataType.Type.DefaultValue();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 指示是否将结果设置为返回类型的默认值 
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <returns></returns>
        protected abstract bool CanUseDefaultValue(HttpResponseMessage responseMessage);
    }
}
