using System.Net;
using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示响应状态为204时将结果设置为返回类型的默认值特性
    /// </summary> 
    public sealed class NoneReturnAttribute : DefaultValueReturnAttribute
    {
        /// <summary>
        /// 响应状态为204时将结果设置为返回类型的默认值特性
        /// </summary>
        public NoneReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 响应状态为204时将结果设置为返回类型的默认值特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public NoneReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 指示是否将结果设置为返回类型的默认值 
        /// </summary>
        /// <param name="responseMessage">响应消息</param>
        /// <returns></returns>
        protected override bool CanUseDefaultValue(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            return responseMessage.IsSuccessStatusCode && responseMessage.Content.Headers.ContentLength == 0;
        }
    }
}
