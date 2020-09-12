using System;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示token应用特性
    /// 需要注册services.AddPasswordCredentialsTokenProvider
    /// </summary> 
    [Obsolete("请使用OAuthTokenAttribute替换")]
    public class PasswordCredentialsTokenAttribute : OAuthTokenAttribute
    {
    }
}
