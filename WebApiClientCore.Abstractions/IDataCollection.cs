using System.Diagnostics.CodeAnalysis;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义数据集合的接口
    /// </summary>
    public interface IDataCollection
    {
        /// <summary>
        /// 获取记录条数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 返回是否包含指定的 key
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool ContainsKey(object key);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void Set(object key, object? value);

        /// <summary>
        /// 读取指定的键并尝试转换为目标类型
        /// 失败则返回目标类型的 default 值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        [return: MaybeNull]
        TValue Get<TValue>(object key);

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool TryGetValue(object key, out object? value);

        /// <summary>
        /// 尝试获取值并转换为TValue类型
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool TryGetValue<TValue>(object key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// 尝试移除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool TryRemove(object key, out object? value);
    }
}