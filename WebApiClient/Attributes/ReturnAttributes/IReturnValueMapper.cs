using WebApiClient.Contexts;

namespace WebApiClient.Attributes.ReturnAttributes
{
    /// <summary>回复内容转返回值转换器</summary>
    public interface IReturnValueMapper
    {
        /// <summary>回复内容转换成返回值</summary>
        /// <param name="responseValue">回复内容</param>
        /// <param name="context">请求Api的上下文</param>
        /// <returns>返回转换后的对象</returns>
        object Map(object responseValue, ApiActionContext context);
    }
}
