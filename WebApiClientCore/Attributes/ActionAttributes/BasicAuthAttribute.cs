using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore.Parameters;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求的基本授权
    /// </summary>
    [DebuggerDisplay("{baiscAuth}")]
    public class BasicAuthAttribute : ApiActionAttribute
    {
        private readonly string userName;
        private readonly string password;

        /// <summary>
        /// 基本授权
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicAuthAttribute(string userName, string password)
        {
            this.userName = userName ?? throw new ArgumentNullException(userName);
            this.password = password;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var parameter = BasicAuth.GetParameter(this.userName, this.password);
            context.HttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", parameter);
            return Task.CompletedTask;
        }
    }
}
