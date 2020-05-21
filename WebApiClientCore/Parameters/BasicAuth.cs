using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace WebApiClientCore.Parameters
{
    /// <summary>
    /// 表示将自身作为请求的基本授权
    /// </summary>
    public class BasicAuth : Authorization
    {
        /// <summary>
        /// 基本授权
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BasicAuth(string userName, string? password)
            : base("Basic", GetParameter(userName, password))
        {
        }

        /// <summary>
        /// 获取基础认证的参数
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static string GetParameter(string userName, string? password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var value = $"{userName}:{password}";
            var bytes = Encoding.ASCII.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
    }
}
