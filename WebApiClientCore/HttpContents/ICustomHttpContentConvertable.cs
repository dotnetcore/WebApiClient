using System.Net.Http;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 定义支持转换为自定义HttpConent的接口
    /// </summary>
    interface ICustomHttpContentConvertable
    {
        /// <summary>
        /// 转换为自定义内容的HttpContent
        /// </summary>
        /// <returns></returns>
        HttpContent ToCustomHttpContext();
    }
}
