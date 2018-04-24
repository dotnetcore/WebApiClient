using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.Parameterables;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示请求的基本授权
    /// </summary>
    [DebuggerDisplay("{baiscAuth}")]
    public class BasicAuthAttribute : ApiActionAttribute
    {
        /// <summary>
        /// BasicAuth
        /// </summary>
        private readonly BasicAuth baiscAuth;

        /// <summary>
        /// 基本授权
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicAuthAttribute(string userName, string password)
        {
            this.baiscAuth = new BasicAuth(userName, password);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            return this.baiscAuth.BeforeRequestAsync(context, null);
        }
    }
}
