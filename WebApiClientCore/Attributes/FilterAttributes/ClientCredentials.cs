using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Client身份信息
    /// </summary>
    public class ClientCredentials
    {
        /// <summary>
        /// 获取或设置提供Token获取的Url节点
        /// 必填
        /// </summary>
        [Required]
        public Uri TokenEndpoint { get; set; }

        /// <summary>
        /// 获取或设置client_id
        /// 必填
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// 获取或设置client_secret
        /// 必填
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }

        /// <summary>
        /// 获取或设置资源范围
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 获取或设置额外字段，支持字典或模型
        /// </summary>
        public object Extra { get; set; }
    }
}
