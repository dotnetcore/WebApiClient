using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 任务扩展
    /// </summary>
    public static class TaskExtenstions
    {
        /// <summary>
        /// 当请求异常时返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<T> ExceptionThenDefault<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
