using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 表示Client身份信息选项
    /// </summary>
    public class ClientCredentialsOptions
    {
        /// <summary>
        /// 获取或设置提供Token获取的Url节点       
        /// </summary>
        [Required]
        [DisallowNull]
        public Uri? Endpoint { get; set; }

        /// <summary>
        /// 是否尝试使用token刷新功能
        /// 禁用则token过期时总是去请求新token
        /// </summary>
        public bool UseRefreshToken { get; set; } = true;

        /// <summary>
        /// 获取或设置Client身份信息
        /// </summary>
        [Required]
        public ClientCredentials Credentials { get; set; } = new ClientCredentials();
    }
}
