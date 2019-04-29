using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient.DataAnnotations;
using WebApiClient.Defaults;

namespace WebApiClient.AuthTokens
{
    /// <summary>
    /// 表示提供token获取功能的客户端
    /// </summary>
    public class TokenClient
    {
        /// <summary>
        /// json序列化工具
        /// </summary>
        private static readonly IJsonFormatter jsonFormatter = new JsonFormatter();

        /// <summary>
        /// keyValue序列化工具
        /// </summary>
        private static readonly IKeyValueFormatter keyValueFormatter = new KeyValueFormatter();


        /// <summary>
        /// 请求超时时间
        /// </summary>
        private TimeSpan timeout = TimeSpan.FromSeconds(30d);

        /// <summary>
        /// 获取或设置请求超时时间
        /// 默认为30s
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan TimeOut
        {
            get
            {
                return this.timeout;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(TimeOut));
                }
                this.timeout = value;
            }
        }

        /// <summary>
        /// 获取提供Token获取的Url节点
        /// </summary>
        public Uri TokenEndpoint { get; }

        /// <summary>
        /// 获取序列化选项
        /// </summary>
        public FormatOptions FormatOptions { get; } = new FormatOptions();


        /// <summary>
        /// 提供token获取功能的客户端
        /// </summary>
        /// <param name="tokenEndpoint">提供Token获取的Url节点</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public TokenClient(string tokenEndpoint)
            : this(new Uri(tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint))))
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
        public Task<TokenResult> RequestClientCredentialsAsync(
            string client_id,
            string client_secret,
            string scope = null,
            object extra = null)
        {
            var credentials = new Credentials
            {
                GrantType = "client_credentials",
                ClientId = client_id,
                ClientSecret = client_secret,
                Scope = scope,
                Extra = extra
            };
            return this.RequestTokenResultAsync(credentials);
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
        public Task<TokenResult> RequestPasswordCredentialsAsync(
            string client_id,
            string client_secret,
            string username,
            string password,
            string scope = null,
            object extra = null)
        {
            var credentials = new Credentials
            {
                GrantType = "password",
                ClientId = client_id,
                ClientSecret = client_secret,
                Username = username,
                Password = password,
                Scope = scope,
                Extra = extra
            };
            return this.RequestTokenResultAsync(credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="client_id">客户端id</param>
        /// <param name="client_secret">客户端秘钥</param>
        /// <param name="refresh_token">获取token得到的refresh_token</param>
        /// <param name="extra">额外字段，支持字典或模型</param>
        /// <returns></returns>
        public Task<TokenResult> RequestRefreshTokenAsync(
            string client_id,
            string client_secret,
            string refresh_token,
            object extra = null)
        {
            var credentials = new Credentials
            {
                GrantType = "refresh_token",
                ClientId = client_id,
                ClientSecret = client_secret,
                RefreshToken = refresh_token,
                Extra = extra
            };
            return this.RequestTokenResultAsync(credentials);
        }

        /// <summary>
        /// 请求Token
        /// </summary>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        private async Task<TokenResult> RequestTokenResultAsync(Credentials credentials)
        {
            var httpContent = new UrlEncodedContent();
            var keyValues = keyValueFormatter.Serialize(nameof(credentials), credentials, this.FormatOptions);
            await httpContent.AddFormFieldAsync(keyValues).ConfigureAwait(false);

            var handler = new DefaultHttpClientHandler();
            using (var httpClient = new HttpClient(handler, true) { Timeout = this.timeout })
            {
                var response = await httpClient.PostAsync(this.TokenEndpoint, httpContent).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var token = jsonFormatter.Deserialize(json, typeof(TokenResult));
                return token as TokenResult;
            }
        }

        /// <summary>
        /// 身份信息
        /// </summary>
        private class Credentials
        {
            [AliasAs("grant_type")]
            public string GrantType { get; set; }

            [AliasAs("client_id")]
            public string ClientId { get; set; }

            [AliasAs("client_secret")]
            public string ClientSecret { get; set; }

            [IgnoreWhenNull]
            [AliasAs("username")]
            public string Username { get; set; }

            [IgnoreWhenNull]
            [AliasAs("password")]
            public string Password { get; set; }

            [IgnoreWhenNull]
            [AliasAs("scope")]
            public string Scope { get; set; }

            [IgnoreWhenNull]
            [AliasAs("refresh_token")]
            public string RefreshToken { get; set; }

            [IgnoreWhenNull]
            [AliasAs("extra")]
            public object Extra { get; set; }
        }
    }
}
