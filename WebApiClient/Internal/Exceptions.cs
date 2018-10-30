using System;

namespace WebApiClient
{
    /// <summary>
    /// 提供委托的单一异常类型捕获
    /// </summary>
    static class Exceptions
    {
        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action">操作</param>     
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TException Catch<TException>(Action action) where TException : Exception
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action.Invoke();
                return default(TException);
            }
            catch (TException ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action">操作</param>    
        /// <param name="onException">异常回调</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Catch<TException>(Action action, Action<TException> onException) where TException : Exception
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action.Invoke();
            }
            catch (TException ex)
            {
                onException?.Invoke(ex);
            }
        }
    }
}
