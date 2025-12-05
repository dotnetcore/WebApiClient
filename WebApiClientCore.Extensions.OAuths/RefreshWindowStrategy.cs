namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示Token刷新窗口计算策略
    /// </summary>
    public enum RefreshWindowStrategy
    {
        /// <summary>
        /// 仅使用固定秒数
        /// </summary>
        FixedSeconds = 0,

        /// <summary>
        /// 仅使用百分比
        /// </summary>
        Percentage = 1,

        /// <summary>
        /// 自动选择：取固定秒数和百分比的较小值
        /// </summary>
        Auto = 2
    }
}
