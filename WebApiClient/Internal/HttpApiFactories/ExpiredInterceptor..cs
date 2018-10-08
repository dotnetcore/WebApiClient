using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示过期状态的拦截器
    /// </summary>
    class ExpiredInterceptor : IDisposable
    {
        /// <summary>
        /// 用于释放资源的对象
        /// </summary>
        private readonly IDisposable disposable;

        /// <summary>
        /// 监视对象的弱引用
        /// </summary>
        private readonly WeakReference weakReference;

        /// <summary>
        /// 获取是否可以释放资源
        /// </summary>
        /// <returns></returns>
        public bool CanDispose
        {
            get => this.weakReference.IsAlive == false;
        }

        /// <summary>
        /// 过期状态的拦截器
        /// </summary>
        /// <param name="active">激活状态的拦截器</param>
        public ExpiredInterceptor(ActiveInterceptor active)
        {
            this.disposable = active.HttpApiConfig;
            this.weakReference = new WeakReference(active);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.CanDispose == true)
            {
                this.disposable.Dispose();
            }
        }
    }
}
