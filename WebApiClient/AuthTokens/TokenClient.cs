using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.DataAnnotations;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示提供token获取功能的客户端
    /// </summary>
    public class TokenClient
    {
        /// <summary>
        /// 获取提供Token获取的Url节点
        /// </summary>
        public Uri TokenEndpoint { get; private set; }

        /// <summary>
        /// 提供token获取功能的客户端
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public TokenClient(string tokenEndpoint)
            : this(new Uri(tokenEndpoint))
        {
        }

        /// <summary>
        /// 创建ITokenClient的实例
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TokenClient(Uri tokenEndpoint)
        {
            if (tokenEndpoint == null)
            {
                throw new ArgumentNullException(nameof(tokenEndpoint));
            }

            if (tokenEndpoint.IsAbsoluteUri == false)
            {
                throw new ArgumentException($"{nameof(tokenEndpoint)}要求为绝对Uri");
            }
            this.TokenEndpoint = tokenEndpoint;
        }

        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端秘钥</param>
        /// <param name="scope">资源范围</param>
        /// <param name="extra">额外字段，支持字典或模型</param>
        /// <returns></returns>     
        public async Task<TokenResult> RequestClientCredentialsAsync(
            string client_id,
            string client_secret,
            string scope = null,
            object extra = null)
        {
            var credentials = new Credentials
            {
                grant_type = "client_credentials",
                client_id = client_id,
                client_secret = client_secret,
                scope = scope,
                extra = extra
            };
            return await this.RequestTokenResultAsync(credentials).ConfigureAwait(false);
        }

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
        public async Task<TokenResult> RequestPasswordCredentialsAsync(
            string client_id,
            string client_secret,
            string username,
            string password,
            string scope = null,
            object extra = null)
        {
            var credentials = new Credentials
            {
                grant_type = "password",
                client_id = client_id,
                client_secret = client_secret,
                username = username,
                password = password,
                scope = scope,
                extra = extra
            };
            return await this.RequestTokenResultAsync(credentials).ConfigureAwait(false);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端秘钥</param>
        /// <param name="refresh_token">获取token得到的refresh_token</param>
        /// <param name="extra">额外字段，支持字典或模型</param>
        /// <returns></returns>
        public async Task<TokenResult> RequestRefreshTokenAsync(
            string client_id,
            string client_secret,
            string refresh_token,
            object extra = null)
        {
            var credentials = new Credentials
            {
                grant_type = "refresh_token",
                client_id = client_id,
                client_secret = client_secret,
                refresh_token = refresh_token,
                extra = extra
            };
            return await this.RequestTokenResultAsync(credentials).ConfigureAwait(false);
        }

        /// <summary>
        /// 请求Token
        /// </summary>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        private async Task<TokenResult> RequestTokenResultAsync(Credentials credentials)
        {
            var keyValues = HttpApiConfig
                .DefaultKeyValueFormatter
                .Serialize(null, credentials, null);

            var httpContent = new UrlEncodedContent(null);
            await httpContent.AddFormFieldAsync(keyValues).ConfigureAwait(false);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(this.TokenEndpoint, httpContent).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var token = HttpApiConfig.DefaultJsonFormatter.Deserialize(json, typeof(TokenResult));
                return token as TokenResult;
            }
        }

        /// <summary>
        /// 身份信息
        /// </summary>
        private class Credentials
        {
            public string grant_type { get; set; }

            public string client_id { get; set; }

            public string client_secret { get; set; }

            [IgnoreWhenNull]
            public string username { get; set; }

            [IgnoreWhenNull]
            public string password { get; set; }

            [IgnoreWhenNull]
            public string scope { get; set; }

            [IgnoreWhenNull]
            public string refresh_token { get; set; }

            [IgnoreWhenNull]
            public object extra { get; set; }
        }
    }
}
