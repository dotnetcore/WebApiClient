using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求的基本授权
    /// </summary>
    [DebuggerDisplay("{Basic} {parameter}")]
    public class BasicAuthAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 授权头的值
        /// </summary>
        private readonly BasicAuthenticationHeaderValue authorization;

        /// <summary>
        /// 基本授权
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicAuthAttribute(string username, string? password)
        {
            this.authorization = new BasicAuthenticationHeaderValue(username, password);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            context.HttpContext.RequestMessage.Headers.Authorization = authorization;
            return Task.CompletedTask;
        }
    }
}
