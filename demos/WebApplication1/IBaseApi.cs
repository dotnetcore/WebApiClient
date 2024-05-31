using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.Exceptions;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace WebApplication1
{
    /// <summary>
    /// 先运行 WebApplication2 作为 API服务器
    /// </summary>
    [OAuthToken, LoggingFilter, HttpHost("https://localhost:7150/api/")]
    public interface ITestApi : IHttpApi
    {
        [HttpGet("todos/test/{id}")]
        Task<HttpResponseMessage> GetAsync([OAuthTokenKey] string app_id, [PathQuery] string id);

        [OAuthToken(Enable = false), HttpPost("todos/token")]
        Task<TokenResult> GetTokenAsync([FormField] string app_id, [FormField] string app_secret);
    }


    public class TestDynamicProvider(IServiceProvider services, ITestApi testApi)
        : TokenProvider(services)
    {
        private record AppClient(string ClientId, string AppId, string AppSecret);

        //模拟配置多用户信息：10个APP
        private static AppClient[] Apps { get; set; }
            = Enumerable.Range(1, 10)
                .Select(p => new AppClient($"client{p}", $"app{p}", $"secret{p}"))
                .ToArray();

        protected override async Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider, string? key)
        {
            var client = Apps.FirstOrDefault(p => p.AppId == key)
                ?? throw new TokenIdentifierException($"未找到标识为`{key}`的应用凭证");

            return await testApi.GetTokenAsync(client.AppId, client.AppSecret);
        }

        protected override Task<TokenResult?> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
        {
            throw new NotImplementedException();
        }

        protected override Task<TokenResult?> RequestTokenAsync(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }

}
