using System;

namespace WebApiClient
{
    /// <summary>
    /// 提供委托的单一异常类型捕获
    /// </summary>
    static class Try
    {
        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action">操作</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Catch<TException>(Action action) where TException : Exception
        {
            Catch<TException>(action, null);
        }

        /// <summary>
        /// 捕获异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action">操作</param>
        /// <param name="exception">异常</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Catch<TException>(Action action, Action<TException> exception) where TException : Exception
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
                exception?.Invoke(ex);
            }
        }
    }
}
