using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.OAuths
{
    /// <summary>
    /// 表示Token提供者抽象类
    /// </summary>
    public abstract class TokenProvider : IDisposable
    {
        /// <summary>
        /// 最近请求到的token
        /// </summary>
        private TokenResult token;

        /// <summary>
        /// 异步锁
        /// </summary>
        private readonly AsyncRoot asyncRoot = new AsyncRoot();

        /// <summary>
        /// 获取token信息
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        public async Task<TokenResult> GetTokenAsync(HttpContext context)
        {
            using (await this.asyncRoot.LockAsync().ConfigureAwait(false))
            {
                await this.InitOrRefreshTokenAsync(context).ConfigureAwait(false);
            }
            return this.token;
        }

        /// <summary>
        /// 初始化或刷新token
        /// </summary>
        /// <exception cref="HttpApiTokenException"></exception>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task InitOrRefreshTokenAsync(HttpContext context)
        {
            if (this.token == null)
            {
                var oathClient = this.CreateOAuthClient(context);
                this.token = await this.RequestTokenAsync(oathClient).ConfigureAwait(false);
            }
            else if (this.token.IsExpired() == true)
            {
                var oathClient = this.CreateOAuthClient(context);
                this.token = this.token.CanRefresh() == true
                    ? await this.RefreshTokenAsync(oathClient, this.token.Refresh_token).ConfigureAwait(false)
                    : await this.RequestTokenAsync(oathClient).ConfigureAwait(false);
            }

            if (this.token == null)
            {
                throw new HttpApiTokenException(Resx.cannot_GetToken);
            }
            this.token.EnsureSuccess();
        }

        /// <summary>
        /// 创建用于请求Token的客户端
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IOAuthClient CreateOAuthClient(HttpContext context)
        {
            var options = new HttpApiOptions();
            options.KeyValueSerializeOptions.IgnoreNullValues = true;
            return HttpApi.Create<IOAuthClient>(context.Client, context.Services, options);
        }


        /// <summary>
        /// 请求获取token
        /// </summary>
        /// <param name="oauthClient"></param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RequestTokenAsync(IOAuthClient oauthClient);


        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="oauthClient"></param> 
        /// <param name="refresh_token">刷新token</param>
        /// <returns></returns>
        protected abstract Task<TokenResult> RefreshTokenAsync(IOAuthClient oauthClient, string refresh_token);


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.asyncRoot.Dispose();
        }
    }
}
