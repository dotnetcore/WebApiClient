using System;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiFactory的接口
    /// </summary>
    interface _IHttpApiFactory
    {
        /// <summary>
        /// 获取生命周期
        /// </summary>
        TimeSpan Lifetime { get; }

        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <returns></returns>
        object CreateHttpApi();

        /// <summary>
        /// 当有记录失效时
        /// </summary>
        /// <param name="entry">激活状态的记录</param>
        void OnEntryDeactivate(ActiveEntry entry);
    }
}
