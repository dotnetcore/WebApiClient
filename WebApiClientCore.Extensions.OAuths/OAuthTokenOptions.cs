namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示OAuth Token的刷新选项
    /// </summary>
    public class OAuthTokenOptions
    {
        /// <summary>
        /// 获取或设置是否启用Token提前刷新窗口
        /// 默认值为 true
        /// </summary>
        public bool UseTokenRefreshWindow { get; set; } = true;

        /// <summary>
        /// 获取或设置固定刷新窗口时长（秒）
        /// 当剩余有效时间小于等于此值时，触发提前刷新
        /// 默认值为 60 秒
        /// </summary>
        public int RefreshWindowSeconds { get; set; } = 60;

        /// <summary>
        /// 获取或设置刷新窗口百分比（0-1）
        /// 当剩余有效时间小于等于总有效期的此百分比时，触发提前刷新
        /// 默认值为 0.1 (10%)
        /// </summary>
        public double RefreshWindowPercentage { get; set; } = 0.1;

        /// <summary>
        /// 获取或设置刷新窗口计算策略
        /// 默认值为 Auto (自动选择固定时长和百分比中的较小值)
        /// </summary>
        public RefreshWindowStrategy RefreshWindowStrategy { get; set; } = RefreshWindowStrategy.Auto;
    }
}
