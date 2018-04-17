using WebApiClient.Attributes;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示Token客户端接口
    /// </summary>
    [JsonReturn]
    public interface ITokenToken
    {
        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端秘钥</param>
        /// <param name="scope">资源范围</param>
        /// <param name="extra">额外字段，支持字典或模型</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "client_credentials")]
        ITask<TokenResult> RequestClientCredentialsAsync(
           [FormField] string client_id,
           [FormField] string client_secret,
           [FormField(IgnoreWhenNull = true)] string scope = null,
           [FormContent(IgnoreWhenNull = true)] object extra = null);

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端秘钥</param>
        /// <param name="username">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="scope">资源范围</param>
        /// <param name="extra">额外字段，支持字典或模型</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "password")]
        ITask<TokenResult> RequestPasswordCredentialsAsync(
          [FormField] string client_id,
          [FormField] string client_secret,
          [FormField] string username,
          [FormField] string password,
          [FormField(IgnoreWhenNull = true)] string scope = null,
          [FormContent(IgnoreWhenNull = true)] object extra = null);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="refresh_token">获取token得到的refresh_token</param>
        /// <param name="extra">额外字段，支持字典或模型</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "refresh_token")]
        ITask<TokenResult> RequestRefreshTokenAsync(
            [FormField] string client_id,
            [FormField]string refresh_token,
            [FormContent(IgnoreWhenNull = true)] object extra = null);
    }
}