using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示结果与条件不匹配的异常
    /// </summary>
    public class ResultNotMatchException : Exception
    {
        /// <summary>
        /// 结果与条件不匹配的异常
        /// </summary>
        /// <param name="message">提示</param>
        public ResultNotMatchException()
            : base("结果与条件不匹配")
        {
        }

        /// <summary>
        /// 结果与条件不匹配的异常
        /// </summary>
        /// <param name="message">提示</param>
        public ResultNotMatchException(string message)
            : base(message)
        {
        }
    }
}
