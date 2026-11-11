using System;
using System.Text.RegularExpressions;

namespace WebApiClientCore.Masking.Strategies
{
    /// <summary>
    /// 表示手机号的脱敏策略
    /// 13812345678 脱敏为 138****5678
    /// </summary>
    public sealed class PhoneMaskStrategy : IMaskStrategy
    {
        /// <summary>
        /// 获取内置策略的名称
        /// </summary>
        public const string StrategyName = "Phone";

        private static readonly Regex phoneRegex = new(
            pattern: @"^(\d{3})\d{4}(\d{4})$",
            options: RegexOptions.CultureInvariant,
            matchTimeout: TimeSpan.FromSeconds(1));

        /// <summary>
        /// 获取策略名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 手机号的脱敏策略
        /// </summary>
        /// <param name="name">策略名称，默认为 Phone</param>
        public PhoneMaskStrategy(string? name = null)
        {
            this.Name = string.IsNullOrWhiteSpace(name) == true ? StrategyName : name!;
        }

        /// <summary>
        /// 对 11 位手机号进行脱敏，不符合格式时返回原值
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public string Mask(string value)
        {
            return phoneRegex.IsMatch(value) == true
                ? phoneRegex.Replace(value, "$1****$2")
                : value;
        }
    }
}
