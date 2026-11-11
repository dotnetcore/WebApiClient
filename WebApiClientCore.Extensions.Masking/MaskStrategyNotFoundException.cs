using System;

namespace WebApiClientCore.Masking
{
    /// <summary>
    /// 表示未找到指定名称的脱敏策略的异常
    /// </summary>
    public sealed class MaskStrategyNotFoundException : Exception
    {
        /// <summary>
        /// 获取未找到的策略名称
        /// </summary>
        public string StrategyName { get; }

        /// <summary>
        /// 未找到指定名称的脱敏策略的异常
        /// </summary>
        /// <param name="strategyName">策略名称</param>
        public MaskStrategyNotFoundException(string strategyName)
            : base($"未找到名称为 '{strategyName}' 的脱敏策略，请先将其注册到 {nameof(MaskStrategyRegistry)}")
        {
            this.StrategyName = strategyName;
        }
    }
}
