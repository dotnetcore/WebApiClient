using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.AuthTokens;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.AuthTokens
{
    public class AuthTokenFilterTest
    {
        class TokenFilter : AuthTokenFilter
        {
            public bool IsRequestTokenResult { get; set; } = false;

            public bool IsRequestRefreshToken { get; set; } = false;

            public TokenResult TokenResult { get; set; }

            protected override Task<TokenResult> RequestTokenResultAsync()
            {
                this.IsRequestTokenResult = true;
                var token = new TokenResult
                {
                    IdToken = "0",
                    ExpiresIn = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    TokenType = "client_credentials"
                };
                return Task.FromResult(token);
            }

            protected override Task<TokenResult> RequestRefreshTokenAsync(string refresh_token)
            {
                this.IsRequestRefreshToken = true;
                var token = new TokenResult
                {
                    IdToken = "1",
                    ExpiresIn = 1,
                    AccessToken = "AccessToken",
                    RefreshToken = "RefreshToken",
                    TokenType = "client_credentials"
                };
                return Task.FromResult(token);
            }

            protected override void AccessTokenResult(ApiActionContext context, TokenResult tokenResult)
            {
                this.TokenResult = tokenResult;
            }
        }

        [Fact]
        public async Task TokenTestAsync()
        {
            var tokenFilter = new TokenFilter();
            IApiActionFilter filter = tokenFilter;

            await filter.OnBeginRequestAsync(null);
            Assert.True(tokenFilter.IsRequestTokenResult && tokenFilter.IsRequestRefreshToken == false && tokenFilter.TokenResult.IdToken == "0");
            await filter.OnEndRequestAsync(null);
            tokenFilter.IsRequestTokenResult = false;
            tokenFilter.IsRequestRefreshToken = false;

            await Task.Delay(500);
            await filter.OnBeginRequestAsync(null);
            Assert.True(tokenFilter.IsRequestTokenResult == false && tokenFilter.IsRequestRefreshToken == false && tokenFilter.TokenResult.IdToken == "0");
            await filter.OnEndRequestAsync(null);
            tokenFilter.IsRequestTokenResult = false;
            tokenFilter.IsRequestRefreshToken = false;


            await Task.Delay(1100);
            await filter.OnBeginRequestAsync(null);
            Assert.True(tokenFilter.IsRequestTokenResult == false && tokenFilter.IsRequestRefreshToken && tokenFilter.TokenResult.IdToken == "1");
            await filter.OnEndRequestAsync(null);
        }
    }
}
