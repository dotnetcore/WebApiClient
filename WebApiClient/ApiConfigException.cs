using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示接口配置异常
    /// </summary>
    public class ApiConfigException : Exception
    {
        /// <summary>
        /// 接口配置异常
        /// </summary>
        /// <param name="message">提示信息</param>
        public ApiConfigException(string message) :
            base(message)
        {
        }
    }
}
