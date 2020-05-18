using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiClientCore.OAuths
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
        public Uri Endpoint { get; set; }

        /// <summary>
        /// 获取或设置Client身份信息
        /// </summary>
        public ClientCredentials Credentials { get; set; }
    }

    /// <summary>
    /// 表示Client身份信息选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientCredentialsOptions<T> : ClientCredentialsOptions
    {
    }
}
