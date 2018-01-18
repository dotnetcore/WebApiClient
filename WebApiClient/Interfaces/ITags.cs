using System;
using WebApiClient.Contexts;

namespace WebApiClient.Interfaces
{
    /// <summary>
    /// 定义请求上下文的数据操作的接口
    /// </summary>
    public interface ITags
    {
        /// <summary>
        /// 获取或设置唯一标识符
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        TagItem this[string key] { get; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        TagItem Get(string key);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void Set(string key, object value);

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Remove(string key);
    }
}
