using System;
using WebApiClientCore.Masking.Strategies;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示需要脱敏的字段声明
    /// 配合 <see cref="MaskedLoggingFilterAttribute"/> 使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class MaskFieldAttribute : Attribute
    {
        /// <summary>
        /// 获取字段名称或路径
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// 获取或设置脱敏策略的名称
        /// </summary>
        public string StrategyName { get; set; } = ReplaceMaskStrategy.StrategyName;

        /// <summary>
        /// 获取或设置是否忽略字段名的大小写
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// 需要脱敏的字段声明
        /// </summary>
        /// <param name="fieldName">
        /// 字段名称或路径。
        /// 不包含点时匹配任意层级的同名字段；
        /// 包含点时按层级精确匹配，其中 * 表示匹配任意一段
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        public MaskFieldAttribute(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName) == true)
            {
                throw new ArgumentException($"'{nameof(fieldName)}' 不能为空", nameof(fieldName));
            }

            this.FieldName = fieldName.Trim();
        }

        /// <summary>
        /// 需要脱敏的字段声明
        /// </summary>
        /// <param name="fieldName">字段名称或路径</param>
        /// <param name="strategyName">脱敏策略的名称</param>
        /// <exception cref="ArgumentException"></exception>
        public MaskFieldAttribute(string fieldName, string strategyName)
            : this(fieldName)
        {
            if (string.IsNullOrWhiteSpace(strategyName) == true)
            {
                throw new ArgumentException($"'{nameof(strategyName)}' 不能为空", nameof(strategyName));
            }

            this.StrategyName = strategyName;
        }
    }
}
