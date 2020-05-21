using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示Client身份信息选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientCredentialsOptions<T>
    {
        /// <summary>
        /// 获取或设置提供Token获取的Url节点       
        /// </summary>
        [Required]
        [DisallowNull]
        public Uri? Endpoint { get; set; }

        /// <summary>
        /// 获取或设置Client身份信息
        /// </summary>
        [Required]
        public ClientCredentials Credentials { get; set; } = new ClientCredentials();
    }
}
