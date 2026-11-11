using System;

namespace WebApiClientCore.Masking.Strategies
{
    /// <summary>
    /// 表示将字段值替换为固定字符串的脱敏策略
    /// </summary>
    public sealed class ReplaceMaskStrategy : IMaskStrategy
    {
        /// <summary>
        /// 获取内置策略的名称
        /// </summary>
        public const string StrategyName = "Replace";

        /// <summary>
        /// 获取替换后的字符串
        /// </summary>
        public string Replacement { get; }

        /// <summary>
        /// 获取策略名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 将字段值替换为固定字符串的脱敏策略
        /// </summary>
        /// <param name="replacement">替换后的字符串</param>
        /// <param name="name">策略名称，默认为 Replace</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ReplaceMaskStrategy(string replacement = "***", string? name = null)
        {
            this.Replacement = replacement ?? throw new ArgumentNullException(nameof(replacement));
            this.Name = string.IsNullOrWhiteSpace(name) == true ? StrategyName : name!;
        }

        /// <summary>
        /// 返回替换后的字符串
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public string Mask(string value)
        {
            return this.Replacement;
        }
    }
}
