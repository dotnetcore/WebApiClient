using System;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为请求uri的特性  
    /// 要求必须修饰于第一个参数
    /// 支持绝对或相对路径
    /// </summary>
    [Obsolete("请使用UriAttribute类型替代", false)]
    public sealed class UrlAttribute : UriAttribute
    {
    }
}
