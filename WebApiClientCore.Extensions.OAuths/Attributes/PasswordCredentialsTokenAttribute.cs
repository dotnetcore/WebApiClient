using System;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示 token 应用特性
    /// 需要注册 services.AddPasswordCredentialsTokenProvider
    /// </summary> 
    [Obsolete("请使用OAuthTokenAttribute替换")]
    public class PasswordCredentialsTokenAttribute : OAuthTokenAttribute
    {
    }
}
