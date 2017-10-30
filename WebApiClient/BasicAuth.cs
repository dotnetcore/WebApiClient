using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClient
{
    /// <summary>
    /// 表示将自身作为请求的基本授权
    /// </summary>
    [DebuggerDisplay("{Authorization} = {authValue}")]
    public class BasicAuth : IApiParameterable
    {
        /// <summary>
        /// 授权的值
        /// </summary>
        private readonly string authValue;

        /// <summary>
        /// secheme
        /// </summary>
        private static readonly string scheme = "Authorization";

        /// <summary>
        /// 基本授权
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicAuth(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            this.authValue = GetBasicAuthValue(userName, password);
        }

        /// <summary>
        /// 获取基础认证的内容
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static string GetBasicAuthValue(string userName, string password)
        {
            var value = string.Format("{0}:{1}", userName, password);
            var bytes = Encoding.ASCII.GetBytes(value);
            return "Basic " + Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var header = context.RequestMessage.Headers;
            header.Remove(scheme);
            if (this.authValue != null)
            {
                header.TryAddWithoutValidation(scheme, this.authValue);
            }
            return ApiTask.CompletedTask;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", scheme, this.authValue);
        }
    }
}
