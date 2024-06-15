using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示OAuth2的Token客户端
    /// </summary>
    sealed class OAuth2TokenClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        private readonly HttpApiOptions httpApiOptions;

        /// <summary>
        /// OAuth2的Token客户端
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="httpApiOptionsMonitor"></param>
        public OAuth2TokenClient(IHttpClientFactory httpClientFactory, IOptionsMonitor<HttpApiOptions> httpApiOptionsMonitor)
        {
            this.httpClientFactory = httpClientFactory;
            this.httpApiOptions = httpApiOptionsMonitor.Get(HttpApi.GetName(typeof(OAuth2TokenClient)));
        }

        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "client_credentials")]
#if NET8_0_OR_GREATER
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
        public Task<TokenResult?> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] ClientCredentials credentials)
        {
            return this.PostFormAsync(endpoint, "client_credentials", credentials);
        }

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "password")]
#if NET8_0_OR_GREATER
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
        public Task<TokenResult?> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] PasswordCredentials credentials)
        {
            return this.PostFormAsync(endpoint, "password", credentials);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "refresh_token")]
#if NET8_0_OR_GREATER
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
        public Task<TokenResult?> RefreshTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] RefreshTokenCredentials credentials)
        {
            return this.PostFormAsync(endpoint, "refresh_token", credentials);
        }

        /// <summary>
        /// POST表单
        /// </summary>
        /// <typeparam name="TCredentials"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="grant_type"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
        private async Task<TokenResult?> PostFormAsync<TCredentials>(Uri endpoint, string grant_type, TCredentials credentials)
        {
            using var formContent = new FormContent(credentials, this.httpApiOptions.KeyValueSerializeOptions);
            formContent.AddFormField(new KeyValue("grant_type", grant_type));

            var response = await this.httpClientFactory.CreateClient().PostAsync(endpoint, formContent);
            var utf8Json = await response.Content.ReadAsUtf8ByteArrayAsync();
            if (utf8Json.Length == 0)
            {
                return default;
            }
            return JsonSerializer.Deserialize<TokenResult>(utf8Json, this.httpApiOptions.JsonDeserializeOptions);
        }
    }
}
