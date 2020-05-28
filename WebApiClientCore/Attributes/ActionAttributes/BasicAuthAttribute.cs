using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
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
        /// 参数
        /// </summary>
        private readonly string parameter;

        /// <summary>
        /// 基本授权
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicAuthAttribute(string userName, string? password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(userName);
            }

            var value = $"{userName}:{password}";
            var bytes = Encoding.ASCII.GetBytes(value);
            this.parameter = Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            context.HttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", this.parameter);
            return Task.CompletedTask;
        }
    }
}
