using System.Text;

namespace System.Net.Http.Headers
{
    /// <summary>
    /// 表示Basic体系的授权头值
    /// </summary>
    public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        /// <summary>
        /// 获取账号
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// 获取密码
        /// </summary>
        public string? Password { get; }

        /// <summary>
        /// Basic体系的授权头值
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        public BasicAuthenticationHeaderValue(string username, string? password)
            : base("Basic", GetParameter(username, password))
        {
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private static string GetParameter(string username, string? password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(username);
            }

            var value = $"{username}:{password}";
            var bytes = Encoding.ASCII.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
    }
}
