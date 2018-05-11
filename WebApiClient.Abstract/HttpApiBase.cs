using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示IHttpApi实现类的抽象类
    /// </summary>
    public abstract class HttpApiBase : IHttpApi
    {
        /// <summary>
        /// 可释放对象
        /// </summary>
        private readonly IDisposable disposable;

        /// <summary>
        /// IHttpApi实现类的抽象类
        /// </summary>
        /// <param name="disposable">可释放对象</param>
        public HttpApiBase(IDisposable disposable)
        {
            this.disposable = disposable;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.disposable?.Dispose();
        }
    }
}
