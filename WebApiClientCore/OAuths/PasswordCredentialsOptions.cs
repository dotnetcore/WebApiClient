using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiClientCore.OAuths
{
    /// <summary>
    /// 表示用户名密码身份信息选项
    /// </summary>
    public class PasswordCredentialsOptions
    {
        /// <summary>
        /// 获取或设置提供Token获取的Url节点       
        /// </summary>
        [Required]
        public Uri Endpoint { get; set; }

        /// <summary>
        /// 获取或设置Client身份信息
        /// </summary>
        public PasswordCredentials Credentials { get; set; }
    }

    /// <summary>
    /// 表示账号密码身份信息选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PasswordCredentialsOptions<T> : PasswordCredentialsOptions
    {
    }
}
